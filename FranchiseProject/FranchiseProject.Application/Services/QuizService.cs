using AutoMapper;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _currentTime;
        private readonly IValidator<CreateQuizModel> _createQuizValidator;
        private readonly IValidator<UpdateQuizModel> _updateQuizValidator;
        private readonly IHubContext<NotificationHub> _hubContext;

        public QuizService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService,
            IValidator<CreateQuizModel> createQuizValidator, ICurrentTime currentTime, 
            IValidator<UpdateQuizModel> updateQuizValidator, IHubContext<NotificationHub> hubContext)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createQuizValidator = createQuizValidator;
            _currentTime = currentTime;
            _updateQuizValidator = updateQuizValidator;
            _hubContext = hubContext;
        }
        public async Task<ApiResponse<QuizStudentViewModel>> GetAllQuizForStudentByQuizId(Guid id)
        {
            var response = new ApiResponse<QuizStudentViewModel>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();

                var quizzes = await _unitOfWork.QuizRepository
                    .GetQuizScoreStudentByQuizIdAndStudentId(id, userId);
                if (quizzes == null) return ResponseHandler.Success(new QuizStudentViewModel(), "Bài kiểm tra không khả dụng!");

                var classs = await _unitOfWork.ClassRepository.GetExistByIdAsync((Guid)quizzes.ClassId);
                if (classs == null || classs.Status != ClassStatusEnum.Active)
                    return ResponseHandler.Success(response.Data, "Lớp học không khả dụng!");

                var quizModel = _mapper.Map<QuizStudentViewModel>(quizzes);
                response = ResponseHandler.Success(quizModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<QuizStudentViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateQuizByIdAsync(Guid quizId, UpdateQuizModel updateQuizModel) 
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateQuizValidator.ValidateAsync(updateQuizModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult); 
                var quiz = await _unitOfWork.QuizRepository.GetExistByIdAsync(quizId);
                if (quiz == null) throw new Exception("Quiz does not exist!");

                quiz = _mapper.Map(updateQuizModel,quiz);
                
                _unitOfWork.QuizRepository.Update(quiz);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Bài kiểm tra được cập nhật thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteQuizByIdAsync(Guid quizId) 
        {
            var response = new ApiResponse<bool>();
            try
            {
                var quiz = await _unitOfWork.QuizRepository.GetExistByIdAsync(quizId);
                if (quiz == null) throw new Exception("Quiz does not exist!");

                _unitOfWork.QuizRepository.SoftRemove(quiz);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Bài kiểm tra được xóa thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> SubmitQuiz(Guid quizId, AnswerModel answerModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var studentId = _claimsService.GetCurrentUserId.ToString();

                var quiz = await _unitOfWork.QuizRepository.GetQuizForStudentById(quizId);
                var checkQuiz = await CheckQuizAvailable(quiz, studentId);

                if (checkQuiz.Data == false) return response;
                if(quiz.QuizDetails == null || !quiz.QuizDetails.Any()) throw new Exception("QuizDetail does not exist!");

                double pointPerQuestion = 10.0 / quiz.QuizDetails.Count();
                double totalScore = 0.0;

                /*foreach (var quizDetail in quiz.QuizDetails)
                {
                    var correctOptionIds = quizDetail.Question.QuestionOptions
                                            .Where(opt => opt.Status == true)
                                            .Select(opt => opt.Id)
                                            .ToList();
                    var studentOptionIds = answerModel.QuestionOptionsId
                                            .Where(id => quizDetail.Question.QuestionOptions.Any(opt => opt.Id == id))
                                            .ToList();

                    if (correctOptionIds.Count == studentOptionIds.Count &&
                        !correctOptionIds.Except(studentOptionIds).Any())
                    {
                        totalScore += pointPerQuestion;
                    }
                }*/
                foreach (var quizDetail in quiz.QuizDetails)
                {
                    var correctOptionIds = quizDetail.Question.QuestionOptions
                                            .Where(opt => opt.Status == true)
                                            .Select(opt => opt.Id)
                                            .ToHashSet();
                    var studentOptionIds = answerModel.QuestionOptionsId
                                            .Where(id => quizDetail.Question.QuestionOptions.Any(opt => opt.Id == id))
                                            .ToHashSet();
                    if (correctOptionIds.SetEquals(studentOptionIds))
                    {
                        totalScore += pointPerQuestion;
                    }
                }
                var score = new Score
                {
                    QuizId = quizId,
                    UserId = studentId,
                    ScoreNumber = totalScore
                };
                await _unitOfWork.ScoreRepository.AddAsync(score);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Submit failed!");

                response = ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<IEnumerable<QuizViewModel>>> GetAllQuizByClassId(Guid id)
        {
            var response = new ApiResponse<IEnumerable<QuizViewModel>>();
            try
            {
                var classs = await _unitOfWork.ClassRepository.GetExistByIdAsync(id);
                if (classs == null || classs.Status != ClassStatusEnum.Active)
                    return ResponseHandler.Success(response.Data , "Lớp học không khả dụng!");

                var quizzes = await _unitOfWork.QuizRepository.GetQuizByClassId(id);
                var quizModel = _mapper.Map<IEnumerable<QuizViewModel>>(quizzes);
                response = ResponseHandler.Success(quizModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<IEnumerable<QuizViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<IEnumerable<QuizStudentViewModel>>> GetAllQuizForStudentByClassId(Guid id)
        {
            var response = new ApiResponse<IEnumerable<QuizStudentViewModel>>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();

                var classs = await _unitOfWork.ClassRepository.GetExistByIdAsync(id);
                if (classs == null || classs.Status != ClassStatusEnum.Active)
                    return ResponseHandler.Success(response.Data, "Lớp học không khả dụng!");

                var quizzes = await _unitOfWork.QuizRepository
                    .GetQuizScoreStudentByClassIdAndStudentId(classs.Id, userId);

                
                var quizModel = _mapper.Map<IEnumerable<QuizStudentViewModel>>(quizzes);
                response = ResponseHandler.Success(quizModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<IEnumerable<QuizStudentViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<QuizDetailStudentViewModel>> GetQuizDetailForStudentByQuizId(Guid id)
        {
            var response = new ApiResponse<QuizDetailStudentViewModel>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();

                var quiz = await _unitOfWork.QuizRepository.GetQuizForStudentById(id);
                var checkQuiz = await CheckQuizAvailable(quiz, userId);
                if (checkQuiz.Data == false && checkQuiz.isSuccess == true)
                    return ResponseHandler.Success(new QuizDetailStudentViewModel(), checkQuiz.Message);
                if (checkQuiz.isSuccess == false)
                    throw new Exception(checkQuiz.Message);

                var quizModel = _mapper.Map<QuizDetailStudentViewModel>(quiz);
                response = ResponseHandler.Success(quizModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<QuizDetailStudentViewModel>(ex.Message);
            }
            return response;
        }

        private async Task<ApiResponse<bool>> CheckQuizAvailable(Quiz quiz, string userId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (quiz == null)
                    return ResponseHandler.Success(false, "Bài kiểm tra không tồn tại!");
                var classs = (await _unitOfWork.ClassRoomRepository
                    .FindAsync(e => e.UserId == userId
                                 && e.ClassId == quiz.ClassId))
                    .FirstOrDefault();
                if (classs == null) return ResponseHandler.Success(false, "Bạn không tồn tại trong danh sách lớp!");

                var score = (await _unitOfWork.ScoreRepository
                    .FindAsync(e => e.UserId == userId &&
                                    e.QuizId == quiz.Id)).FirstOrDefault();
                if(score != null) return ResponseHandler.Success(false, "Bạn đã hoàn thành bài kiểm tra này trước đó!");

                DateTime? checkEndTime = quiz.StartTime?.AddMinutes(quiz.Duration + 5 ?? 0);
                if (checkEndTime == null || quiz.StartTime > _currentTime.GetCurrentTime() ||
                    _currentTime.GetCurrentTime() > checkEndTime)
                    return ResponseHandler.Success(false, "Không nằm trong phạm vi thời gian cho phép!");
                response = ResponseHandler.Success(true);


            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateQuizForClass(CreateQuizModel createQuizModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createQuizValidator.ValidateAsync(createQuizModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var classRoom = (await _unitOfWork.ClassRepository
                    .FindAsync(e => e.Id == createQuizModel.ClassId))
                    .FirstOrDefault();
                if (classRoom == null) throw new Exception("Class does not exist!");
                if (createQuizModel.ChapterId == null) throw new Exception("ChapterId is null!");

                var questions = await _unitOfWork.QuestionRepository
                .FindAsync(e => createQuizModel.ChapterId.Contains((Guid)e.ChapterId), "QuestionOptions");
                var randomQuestions = questions.OrderBy(x => Guid.NewGuid())
                .Take(createQuizModel.Quantity);

                if (randomQuestions.Count() < createQuizModel.Quantity)
                    return ResponseHandler.Success(false,
                        "Số lượng trong ngân hàng câu hỏi không đủ! "
                        + "Số lượng câu hỏi trong ngân hàng: " + randomQuestions.Count());

                var quizdetails = _mapper.Map<List<QuizDetail>>(randomQuestions);

                var quiz = _mapper.Map<Quiz>(createQuizModel);
                quiz.QuizDetails = quizdetails;
                await _unitOfWork.QuizRepository.AddAsync(quiz); 

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                var students = await _unitOfWork.ClassRepository.GetStudentsByClassIdAsync((Guid)createQuizModel.ClassId);
                foreach (var student in students)
                {
                    await _hubContext.Clients.User(student.Id.ToString())
                        .SendAsync("ReceivedNotification", $"Bạn có bài kiểm tra mới bắt đầu lúc {createQuizModel.StartTime.ToString()}.");
                }
                response = ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}
