using AutoMapper;
using ClosedXML.Excel;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.QuestionOptionViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pqc.Crypto.Frodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateQuestionArrangeModel> _createQuestionValidator;
        private readonly IValidator<List<CreateQuestionArrangeModel>> _createQuestionArrangeValidator;
        private readonly IValidator<UpdateQuestionModel> _updateQuestionValidator;
        public QuestionService(IUnitOfWork unitOfWork, ICourseService courseService,
            IValidator<CreateQuestionArrangeModel> createQuestionValidator, IValidator<UpdateQuestionModel> updateQuestionValidator,
        IMapper mapper, IValidator<List<CreateQuestionArrangeModel>> createQuestionArrangeValidator)
        {
            _courseService = courseService;
            _unitOfWork = unitOfWork;
            _createQuestionValidator = createQuestionValidator;
            _mapper = mapper;
            _createQuestionArrangeValidator = createQuestionArrangeValidator;
            _updateQuestionValidator = updateQuestionValidator;
        }
        public async Task<ApiResponse<bool>> CreateQuestionByFileAsync(Guid id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) throw new Exception("File is empty.");
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(id);
                if (course == null) throw new Exception("Course does not exist!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(course.Id, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var chapters = (await _unitOfWork.ChapterRepository
                    .FindAsync(e => e.CourseId == course.Id && e.IsDeleted != true))
                    .OrderBy(e => e.Number)
                    .ToList();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    using (var workbook = new XLWorkbook(stream))
                    {
                        int sheetCount = workbook.Worksheets.Count();
                        if (sheetCount != chapters.Count)
                            return ResponseHandler.Failure<bool>(
                                sheetCount < chapters.Count ? "Số lượng bảng tính ít hơn số lượng chapters trong khoá học." :
                                                              "Số lượng bảng tính nhiều hơn số lượng chapters trong khoá học."
                            );
                        for (var i = 0; i < chapters.Count; i++)
                        {
                            var questionModels = new List<CreateQuestionArrangeModel>();
                            var worksheet = workbook.Worksheets.Skip(i).First();
                            var rows = worksheet.RangeUsed().RowsUsed();
                            var chapterId = chapters[i].Id;

                            foreach (var row in rows.Skip(1))
                            {
                                var questionModel = new CreateQuestionArrangeModel
                                {
                                    Description = row.Cell(1).Value.ToString(),
                                    ImageURL = row.Cell(2).Value.ToString(),
                                    QuestionOptions = new List<CreateQuestionOptionArrangeModel>()
                                };

                                for (var j = 3; j <= row.Cells().Count(); j += 3)
                                {
                                    string cellValue = row.Cell(j + 2).Value.ToString().Trim().ToLowerInvariant();
                                    bool status;

                                    if (cellValue == "true")
                                    {
                                        status = true;
                                    }
                                    else if (cellValue == "false")
                                    {
                                        status = false;
                                    }
                                    else
                                    {
                                        throw new Exception($"Invalid boolean value '{cellValue}' in cell.");
                                    }
                                    var questionOption = new CreateQuestionOptionArrangeModel
                                    {
                                        Description = row.Cell(j).Value.ToString(),
                                        ImageURL = row.Cell(j + 1).Value.ToString(),
                                        Status = status
                                    };
                                    questionModel.QuestionOptions.Add(questionOption);
                                }

                                questionModels.Add(questionModel);
                            }

                            var validationResult = await _createQuestionArrangeValidator.ValidateAsync(questionModels);
                            if (!validationResult.IsValid) throw new Exception(ValidatorHandler.HandleValidation<bool>(validationResult).Message);

                            var questions = _mapper.Map<List<Question>>(questionModels);
                            foreach (var question in questions)
                            {
                                question.ChapterId = chapterId;
                            }
                            await _unitOfWork.QuestionRepository.AddRangeAsync(questions);
                        }
                    }

                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                    if (!isSuccess) throw new Exception("Create failed!");
                }

                return ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>(ex.Message);
            }
        }
        public async Task<ApiResponse<bool>> UpdateQuestionByIdAsync(Guid id, UpdateQuestionModel updateQuestionModel)
        {
            var response = new ApiResponse<bool>();
            try
            {

                ValidationResult validationResult = await _updateQuestionValidator.ValidateAsync(updateQuestionModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var question = await _unitOfWork.QuestionRepository.GetExistByIdAsync(id);
                if (question == null) throw new Exception("Question does not exist!");

                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync((Guid)question.ChapterId);
                if (chapter == null) throw new Exception("Chapter does not exist!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                question = _mapper.Map(updateQuestionModel, question);

                var deleteQuestionOptions = (await _unitOfWork.QuestionOptionRepository.FindAsync(e => e.Question.Id == question.Id && e.IsDeleted != true)).ToList();
                if (!deleteQuestionOptions.IsNullOrEmpty()) _unitOfWork.QuestionOptionRepository.HardRemoveRange(deleteQuestionOptions);

                var questionOptions = _mapper.Map<List<QuestionOption>>(updateQuestionModel.QuestionOptions);
                foreach (var questionOption in questionOptions)
                {
                    questionOption.QuestionId = question.Id;
                }

                await _unitOfWork.QuestionOptionRepository.AddRangeAsync(questionOptions);


                _unitOfWork.QuestionRepository.Update(question);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Cập nhật câu hỏi thành công!");

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateQuestionByChapterIdAsync(Guid chapterId, CreateQuestionArrangeModel createQuestionArrangeModel)
        {
            var response = new ApiResponse<bool>();
            try
            {

                ValidationResult validationResult = await _createQuestionValidator.ValidateAsync(createQuestionArrangeModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync(chapterId);
                if (chapter == null) throw new Exception("Chapter does not exist!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var question = _mapper.Map<Question>(createQuestionArrangeModel);

                question.ChapterId = chapterId;

                await _unitOfWork.QuestionRepository.AddAsync(question);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo câu hỏi thành công!");

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteQuestionByIdAsync(Guid questionId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var question = await _unitOfWork.QuestionRepository.GetExistByIdAsync(questionId);
                if (question == null) throw new Exception("Question does not exist!");

                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync((Guid)question.ChapterId);
                if (chapter == null) throw new Exception("Chapter does not exist!");


                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.QuestionRepository.HardRemove(question);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá câu hỏi của chương học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateQuestionArrangeAsync(Guid chapterId, List<CreateQuestionArrangeModel> createQuestionArrangeModel)
        {
            var response = new ApiResponse<bool>();
            try
            {

                ValidationResult validationResult = await _createQuestionArrangeValidator.ValidateAsync(createQuestionArrangeModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync(chapterId);
                if (chapter == null) throw new Exception("Chapter does not exist!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var questions = _mapper.Map<List<Question>>(createQuestionArrangeModel);
                foreach (var question in questions)
                {
                    question.ChapterId = chapterId;
                }
                var deleteQuestions = (await _unitOfWork.QuestionRepository.FindAsync(e => e.ChapterId == chapterId && e.IsDeleted != true)).ToList();
                if (!deleteQuestions.IsNullOrEmpty()) _unitOfWork.QuestionRepository.SoftRemoveRange(deleteQuestions);

                await _unitOfWork.QuestionRepository.AddRangeAsync(questions);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo câu hỏi thành công!");

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<QuestionViewModel>>> GetAllQuestionByChapterId(Guid chapterId)
        {
            var response = new ApiResponse<List<QuestionViewModel>>();
            try
            {

                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync(chapterId);
                if (chapter == null) throw new Exception("Chapter does not exist!");

                var questions = (await _unitOfWork.QuestionRepository.FindAsync(e => e.ChapterId == chapterId && e.IsDeleted != true, "QuestionOptions")).ToList();
                var questionModels = _mapper.Map<List<QuestionViewModel>>(questions);
                if (questions.Count == 0) return ResponseHandler.Success(questionModels, "Không tồn tại câu hỏi nào");
                response = ResponseHandler.Success(questionModels);

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<QuestionViewModel>>(ex.Message);
            }
            return response;
        }
    }
}
