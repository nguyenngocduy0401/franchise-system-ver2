using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
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
        private readonly IValidator<CreateQuizModel> _createQuizValidator;
        private readonly ICurrentTime _currentTime;

        public QuizService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService,
            IValidator<CreateQuizModel> createQuizValidator, ICurrentTime currentTime)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createQuizValidator = createQuizValidator;
            _currentTime = currentTime;
        }
        public async Task<ApiResponse<IEnumerable<QuizViewModel>>> GetAllQuizByClassId(Guid id)
        {
            var response = new ApiResponse<IEnumerable<QuizViewModel>>();
            try
            {
                var classs = await _unitOfWork.ClassRepository.GetExistByIdAsync(id);
                if (classs == null || classs.Status != ClassStatusEnum.Active)
                    return ResponseHandler.Success(response.Data , "Lớp học không khả dụng!");

                var quizzes = await _unitOfWork.QuizRepository
                    .FindAsync(e => e.ClassId == classs.Id);


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
                if (checkQuiz.Data == false)
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
                                 && e.Status == ClassRoomStatusEnum.Active
                                 && e.ClassId == quiz.ClassId))
                    .FirstOrDefault();
                if (classs == null) return ResponseHandler.Success(false, "Bạn không tồn tại trong danh sách lớp!");
                DateTime? checkEndTime = quiz.StartTime?.AddMinutes(quiz.Duration + 5 ?? 0);
                if (checkEndTime == null &&
                    (quiz.StartTime > _currentTime.GetCurrentTime() ||
                    _currentTime.GetCurrentTime() > checkEndTime))
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
                    .FindAsync(e => e.Id == createQuizModel.ClassId && e.Status == ClassStatusEnum.Active))
                    .FirstOrDefault();
                if (classRoom == null) throw new Exception("Class does not exist!");
                if (createQuizModel.ChapterId == null) throw new Exception("ChapterId is null!");

                var questions = await _unitOfWork.QuestionRepository
                .FindAsync(e => createQuizModel.ChapterId.Contains((Guid)e.ChapterId), "QuestionOptions");
                var randomQuestions = questions.OrderBy(x => Guid.NewGuid())
                .Take(createQuizModel.Quantity);

                if (randomQuestions.Count() < createQuizModel.Quantity)
                    return ResponseHandler.Failure<bool>
                        ("Số lượng trong ngân hàng câu hỏi không đủ! "
                        + "Số lượng câu hỏi trong ngân hàng: " + randomQuestions.Count());

                var quizdetails = _mapper.Map<List<QuizDetail>>(questions);

                var quiz = _mapper.Map<Quiz>(createQuizModel);
                quiz.QuizDetails = quizdetails;

                await _unitOfWork.QuizRepository.AddAsync(quiz);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
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
