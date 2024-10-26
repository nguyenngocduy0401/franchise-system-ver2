using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IValidator<List<CreateQuestionArrangeModel>> _createQuestionArrangeValidator;
        public QuestionService(IUnitOfWork unitOfWork, ICourseService courseService,
            IValidator<List<CreateQuestionArrangeModel>> createQuestionArrangeValidator,
            IMapper mapper)
        {
            _courseService = courseService;
            _unitOfWork = unitOfWork;
            _createQuestionArrangeValidator = createQuestionArrangeValidator;
            _mapper = mapper;
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
                if(questions.Count == 0) return ResponseHandler.Success(questionModels, "Không tồn tại câu hỏi nào");
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
