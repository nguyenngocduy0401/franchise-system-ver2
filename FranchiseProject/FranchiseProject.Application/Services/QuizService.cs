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

        public QuizService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService,
            IValidator<CreateQuizModel> createQuizValidator)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createQuizValidator = createQuizValidator;
        }
        public async Task<ApiResponse<QuizDetailStudentViewModel>> GetQuizForStudentByQuizId(Guid id) 
        {
            var response = new ApiResponse<QuizDetailStudentViewModel>();
            try
            {
                var quiz = await _unitOfWork.QuizRepository.GetQuizForStudentById(id);
                if (quiz == null) 
                    return ResponseHandler.Success(new QuizDetailStudentViewModel(),"Bài kiểm tra không tồn tại!");
                var quizModel = _mapper.Map<QuizDetailStudentViewModel>(quiz);
                response = ResponseHandler.Success(quizModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<QuizDetailStudentViewModel>(ex.Message);
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
    .           FindAsync(e => createQuizModel.ChapterId.Contains((Guid)e.ChapterId),"QuestionOptions");
                var randomQuestions = questions.OrderBy(x => Guid.NewGuid())
                .Take(createQuizModel.Quantity);

                if (randomQuestions.Count() < createQuizModel.Quantity) 
                    return ResponseHandler.Failure<bool>
                        ("Số lượng trong ngân hàng câu hỏi không đủ! " 
                        +"Số lượng câu hỏi trong ngân hàng: "+ randomQuestions.Count());

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
