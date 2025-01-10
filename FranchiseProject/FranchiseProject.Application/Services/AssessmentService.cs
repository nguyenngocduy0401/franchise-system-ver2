using AutoMapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.API.Services;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.AssessmentViewModels.SingleAssessmentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ICourseService _courseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _currentTime;
        private readonly IPdfService _pdfService;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<CreateAssessmentModel> _createAssessmentValidator;
        private readonly IValidator<UpdateAssessmentModel> _updateAssessmentValidator;
        private readonly IValidator<List<CreateAssessmentArrangeModel>> _createAssessmentArrangeValidator;
        public AssessmentService(IUnitOfWork unitOfWork, IMapper mapper, ICourseService courseService,
            IValidator<UpdateAssessmentModel> updateAssessmentValidator, IValidator<CreateAssessmentModel> createAssessmentValidator,
            IValidator<List<CreateAssessmentArrangeModel>> createAssessmentArrangeValidator, IClaimsService claimsService,
            ICurrentTime currentTime, UserManager<User> userManager, IPdfService pdfService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _courseService = courseService;
            _updateAssessmentValidator = updateAssessmentValidator;
            _createAssessmentValidator = createAssessmentValidator;
            _createAssessmentArrangeValidator = createAssessmentArrangeValidator;
            _claimsService = claimsService;
            _currentTime = currentTime;
            _userManager = userManager;
            _pdfService = pdfService;
        }
        public async Task<ApiResponse<bool>> CreateAssessmentAsync(CreateAssessmentModel createAssessmentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createAssessmentValidator.ValidateAsync(createAssessmentModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(createAssessmentModel.CourseId,
                    CourseStatusEnum.Draft
                    );
                if (!checkCourse.Data) return checkCourse;

                var assessment = _mapper.Map<Assessment>(createAssessmentModel);
                await _unitOfWork.AssessmentRepository.AddAsync(assessment);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo đánh giá của khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteAssessmentByIdAsync(Guid assessmentId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var assessment = await _unitOfWork.AssessmentRepository.GetExistByIdAsync(assessmentId);
                if (assessment == null) return ResponseHandler.Success<bool>(false, "Đánh giá của khóa học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(assessment.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.AssessmentRepository.SoftRemove(assessment);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá đánh giá của khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<AssessmentViewModel>> GetAssessmentByIdAsync(Guid assessmentId)
        {
            var response = new ApiResponse<AssessmentViewModel>();
            try
            {
                var assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(assessmentId);
                if (assessment == null) throw new Exception("Assessment does not exist!");
                var assessmentModel = _mapper.Map<AssessmentViewModel>(assessment);
                response = ResponseHandler.Success(assessmentModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<AssessmentViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateAssessmentAsync(Guid assessmentId, UpdateAssessmentModel updateAssessmentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateAssessmentValidator.ValidateAsync(updateAssessmentModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var assessment = await _unitOfWork.AssessmentRepository.GetExistByIdAsync(assessmentId);
                if (assessment == null) return ResponseHandler.Success<bool>(false, "Đánh giá của khóa học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(assessment.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                assessment = _mapper.Map(updateAssessmentModel, assessment);
                await _unitOfWork.AssessmentRepository.AddAsync(assessment);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật đánh giá của khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateAssessmentArangeAsync(Guid courseId, List<CreateAssessmentArrangeModel> createAssessmentArrangeModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createAssessmentArrangeValidator.ValidateAsync(createAssessmentArrangeModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var assessments = _mapper.Map<List<Assessment>>(createAssessmentArrangeModel);
                foreach (var assessment in assessments)
                {
                    assessment.CourseId = courseId;
                }
                var deleteAssessments = (await _unitOfWork.AssessmentRepository.FindAsync(e => e.CourseId == courseId && e.IsDeleted != true)).ToList();
                if (!deleteAssessments.IsNullOrEmpty()) _unitOfWork.AssessmentRepository.HardRemoveRange(deleteAssessments);

                await _unitOfWork.AssessmentRepository.AddRangeAsync(assessments);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo đánh giá khóa học thành công!");

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<AssessmentStudentViewModel>> GetStudentAssessmentByLoginAsync(Guid classId)
        {
            var response = new ApiResponse<AssessmentStudentViewModel>();
            try
            {
                var studentId = _claimsService.GetCurrentUserId.ToString();
                var assessments = await _unitOfWork.AssessmentRepository.GetAssessmentsByClassIdAsync(classId, studentId);
                if (assessments == null || !assessments.Any()) return ResponseHandler.Success(new AssessmentStudentViewModel());
                var course = (await _unitOfWork.CourseRepository.FindAsync(c => c.Id == assessments.FirstOrDefault().CourseId, "Syllabus")).FirstOrDefault();
                if (course == null) return ResponseHandler.Success<AssessmentStudentViewModel>(null, "Khoá học hiện không khả dụng!");
                var classSchedules = await _unitOfWork.ClassScheduleRepository.FindAsync(e => e.ClassId == classId && e.IsDeleted != true);
                var assessmentStudentViewModel = new AssessmentStudentViewModel();
                //điền tên khóa học vào model
                assessmentStudentViewModel.CourseId = course.Id;
                assessmentStudentViewModel.CourseName = course.Name;

                foreach (var assessment in assessments)
                {
                    switch (assessment.Type)
                    {
                        case AssessmentTypeEnum.Assignment:
                            {
                                var assignments = new List<SingleAssignmentViewModel>();
                                double avgAssignmentsScore = 0;

                                if (assessment.Assignments != null && assessment.Assignments.Any())
                                {
                                    double weightPerAssignment = (double)(assessment.Weight / assessment.Quantity);
                                    double totalScore = 0;
                                    foreach (var assignment in assessment.Assignments)
                                    {
                                        var submit = assignment.AssignmentSubmits.FirstOrDefault();
                                        var score = submit?.ScoreNumber ?? 0;
                                        var assignmentModel = new SingleAssignmentViewModel()
                                        {
                                            Score = score,
                                            Title = assignment.Title,
                                            Weight = weightPerAssignment
                                        };
                                        assignments.Add(assignmentModel);
                                        totalScore += score;
                                    }
                                    avgAssignmentsScore = totalScore / assessment.Assignments.Count;
                                }
                                var assessmentAssignmentViewModel = new AssessmentAssignmentViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    Score = avgAssignmentsScore,
                                    Assignments = assignments
                                };
                                assessmentStudentViewModel.AssessmentAssignmentView = assessmentAssignmentViewModel;
                                break;
                            }
                        case AssessmentTypeEnum.Quiz:
                            {
                                var quizs = new List<SingleQuizViewModel>();
                                double avgQuizScore = 0;

                                if (assessment.Quizzes != null && assessment.Quizzes.Any())
                                {
                                    double weightPerQuiz = (double)(assessment.Weight / assessment.Quantity);
                                    double totalScore = 0;
                                    foreach (var quiz in assessment.Quizzes)
                                    {
                                        var submit = quiz.Scores.FirstOrDefault();
                                        var score = submit?.ScoreNumber ?? 0;
                                        var quizModel = new SingleQuizViewModel()
                                        {
                                            Score = score,
                                            Title = quiz.Title,
                                            Weight = weightPerQuiz
                                        };
                                        quizs.Add(quizModel);
                                        totalScore += score;
                                    }
                                    avgQuizScore = totalScore / assessment.Quizzes.Count;
                                }
                                var assessmentQuizViewModel = new AssessmentQuizViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    Score = avgQuizScore,
                                    Quizzes = quizs
                                };
                                assessmentStudentViewModel.AssessmentQuizView = assessmentQuizViewModel;
                                break;
                            }
                        case AssessmentTypeEnum.FinalExam:
                            {
                                var finals = new List<SingleFinalViewModel>();
                                double avgFinalScore = 0;

                                if (assessment.Quizzes != null && assessment.Quizzes.Any())
                                {
                                    double weightPerFinal = (double)(assessment.Weight / assessment.Quantity);
                                    double totalScore = 0;
                                    foreach (var quiz in assessment.Quizzes)
                                    {
                                        var submit = quiz.Scores.FirstOrDefault();
                                        var score = submit?.ScoreNumber ?? 0;
                                        var finalModel = new SingleFinalViewModel()
                                        {
                                            Score = score,
                                            Title = quiz.Title,
                                            Weight = weightPerFinal
                                        };
                                        finals.Add(finalModel);
                                        totalScore += score;
                                    }
                                    avgFinalScore = totalScore / assessment.Quizzes.Count;
                                }
                                var assessmentFinalViewModel = new AssessmentFinalViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    Score = avgFinalScore,
                                    Finals = finals,
                                };
                                assessmentStudentViewModel.AssessmentFinalViewModel = assessmentFinalViewModel;
                                break;
                            }
                        case AssessmentTypeEnum.Attendance:
                            {
                                var totalDaysAttended = (await _unitOfWork.ClassScheduleRepository.FindAsync(e => e.ClassId == classId && e.IsDeleted != true)).Count();
                                var attendedDays = classSchedules.Count();
                                var score = 0;
                                if (totalDaysAttended != 0) score = (attendedDays / totalDaysAttended) * 10;

                                var assessmentAttendanceViewModel = new AssessmentAttendanceViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    AttendedDays = attendedDays,
                                    TotalDaysAttended = totalDaysAttended,
                                    Score = score
                                };
                                assessmentStudentViewModel.AssessmentAttendanceView = assessmentAttendanceViewModel;
                                break;
                            }
                    }

                }
                double totalWeightedScore = 0;
                double totalWeight = 0;

                if (assessmentStudentViewModel.AssessmentAssignmentView != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentAssignmentView.Score * assessmentStudentViewModel.AssessmentAssignmentView.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentAssignmentView.Weight;
                }

                if (assessmentStudentViewModel.AssessmentQuizView != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentQuizView.Score * assessmentStudentViewModel.AssessmentQuizView.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentQuizView.Weight;
                }

                if (assessmentStudentViewModel.AssessmentFinalViewModel != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentFinalViewModel.Score * assessmentStudentViewModel.AssessmentFinalViewModel.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentFinalViewModel.Weight;
                }

                if (assessmentStudentViewModel.AssessmentAttendanceView != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentAttendanceView.Score * assessmentStudentViewModel.AssessmentAttendanceView.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentAttendanceView.Weight;
                }

                assessmentStudentViewModel.AverageScore = totalWeight > 0 ? totalWeightedScore / totalWeight : 0;
                var minAvgMarkToPass = course?.Syllabus?.MinAvgMarkToPass ?? 0;
                assessmentStudentViewModel.MinAvgMarkToPass = minAvgMarkToPass;

                var lastClassSchedule = classSchedules.OrderByDescending(c => c.Date).FirstOrDefault();
                //thời gian hoàn thành khóa học
                assessmentStudentViewModel.TimeCompleted = (DateTime)lastClassSchedule.Date;


                if (lastClassSchedule != null && lastClassSchedule.Date <= _currentTime.GetCurrentTime())

                    if (assessmentStudentViewModel.AverageScore >= minAvgMarkToPass &&
                        assessmentStudentViewModel.AssessmentAttendanceView != null &&
                        assessmentStudentViewModel.AssessmentAttendanceView.Score >= assessmentStudentViewModel.AssessmentAttendanceView.CompletionCriteria &&
                        assessmentStudentViewModel.AssessmentAssignmentView != null &&
                        assessmentStudentViewModel.AssessmentAssignmentView.Score >= assessmentStudentViewModel.AssessmentAssignmentView.CompletionCriteria &&
                        assessmentStudentViewModel.AssessmentQuizView != null &&
                        assessmentStudentViewModel.AssessmentQuizView.Score >= assessmentStudentViewModel.AssessmentQuizView.CompletionCriteria &&
                        assessmentStudentViewModel.AssessmentFinalViewModel != null &&
                        assessmentStudentViewModel.AssessmentFinalViewModel.Score >= assessmentStudentViewModel.AssessmentFinalViewModel.CompletionCriteria)
                    {
                    }
                var classRoom = (await _unitOfWork.ClassRoomRepository.FindAsync(e => e.ClassId == classId)).FirstOrDefault();
                if (classRoom != null && classRoom.Certification != null)
                {
                    assessmentStudentViewModel.Certification = classRoom.Certification;
                }
                else
                {
                    var student = await _userManager.FindByIdAsync(studentId);

                    using (var pdfStream = await _pdfService.FillPdfTemplate(student.FullName, (DateTime)lastClassSchedule.Date, course.Name))
                    {
                        if (pdfStream == null)
                        {
                            return ResponseHandler.Success<AssessmentStudentViewModel>(null, "Không thể tạo file PDF từ template.");
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            await pdfStream.CopyToAsync(memoryStream);
                            byte[] pdfBytes = memoryStream.ToArray();


                            string fileName = $"{student.UserName}_{course.Name}_{lastClassSchedule.Date}";
                            using (var uploadStream = new MemoryStream(pdfBytes))
                            {
                                string firebaseUrl = await _unitOfWork.FirebaseRepository.UploadFileAsync(uploadStream, fileName);
                                classRoom.Certification = firebaseUrl;
                                assessmentStudentViewModel.Certification = firebaseUrl;
                                await _unitOfWork.ClassRoomRepository.UpdatesAsync(classRoom);
                                await _unitOfWork.SaveChangeAsync();
                            }
                        }
                    }
                }
                response = ResponseHandler.Success(assessmentStudentViewModel);
            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<AssessmentStudentViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<AssessmentStudentViewModel>> GetStudentAssessmentByIdAsync(Guid classId, string studentId)
        {
            var response = new ApiResponse<AssessmentStudentViewModel>();
            try
            {
                var assessments = await _unitOfWork.AssessmentRepository.GetAssessmentsByClassIdAsync(classId, studentId);
                if (assessments != null && assessments.Any()) return ResponseHandler.Success(new AssessmentStudentViewModel());
                var course = (await _unitOfWork.CourseRepository.FindAsync(c => c.Id == assessments.FirstOrDefault().CourseId, "Syllabus")).FirstOrDefault();
                if (course == null) return ResponseHandler.Success<AssessmentStudentViewModel>(null, "Khoá học hiện không khả dụng!");
                var classSchedules = await _unitOfWork.ClassScheduleRepository.FindAsync(e => e.ClassId == classId && e.IsDeleted != true);
                var assessmentStudentViewModel = new AssessmentStudentViewModel();
                //điền tên khóa học vào model
                assessmentStudentViewModel.CourseId = course.Id;
                assessmentStudentViewModel.CourseName = course.Name;

                foreach (var assessment in assessments)
                {
                    switch (assessment.Type)
                    {
                        case AssessmentTypeEnum.Assignment:
                            {
                                var assignments = new List<SingleAssignmentViewModel>();
                                double avgAssignmentsScore = 0;

                                if (assessment.Assignments != null && assessment.Assignments.Any())
                                {
                                    double weightPerAssignment = (double)(assessment.Weight / assessment.Quantity);
                                    double totalScore = 0;
                                    foreach (var assignment in assessment.Assignments)
                                    {
                                        var submit = assignment.AssignmentSubmits.FirstOrDefault();
                                        var score = submit?.ScoreNumber ?? 0;
                                        var assignmentModel = new SingleAssignmentViewModel()
                                        {
                                            Score = score,
                                            Title = assignment.Title,
                                            Weight = weightPerAssignment
                                        };
                                        assignments.Add(assignmentModel);
                                        totalScore += score;
                                    }
                                    avgAssignmentsScore = totalScore / assessment.Assignments.Count;
                                }
                                var assessmentAssignmentViewModel = new AssessmentAssignmentViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    Score = avgAssignmentsScore,
                                    Assignments = assignments
                                };
                                assessmentStudentViewModel.AssessmentAssignmentView = assessmentAssignmentViewModel;
                                break;
                            }
                        case AssessmentTypeEnum.Quiz:
                            {
                                var quizs = new List<SingleQuizViewModel>();
                                double avgQuizScore = 0;

                                if (assessment.Quizzes != null && assessment.Quizzes.Any())
                                {
                                    double weightPerQuiz = (double)(assessment.Weight / assessment.Quantity);
                                    double totalScore = 0;
                                    foreach (var quiz in assessment.Quizzes)
                                    {
                                        var submit = quiz.Scores.FirstOrDefault();
                                        var score = submit?.ScoreNumber ?? 0;
                                        var quizModel = new SingleQuizViewModel()
                                        {
                                            Score = score,
                                            Title = quiz.Title,
                                            Weight = weightPerQuiz
                                        };
                                        quizs.Add(quizModel);
                                        totalScore += score;
                                    }
                                    avgQuizScore = totalScore / assessment.Quizzes.Count;
                                }
                                var assessmentQuizViewModel = new AssessmentQuizViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    Score = avgQuizScore,
                                    Quizzes = quizs
                                };
                                assessmentStudentViewModel.AssessmentQuizView = assessmentQuizViewModel;
                                break;
                            }
                        case AssessmentTypeEnum.FinalExam:
                            {
                                var finals = new List<SingleFinalViewModel>();
                                double avgFinalScore = 0;

                                if (assessment.Quizzes != null && assessment.Quizzes.Any())
                                {
                                    double weightPerFinal = (double)(assessment.Weight / assessment.Quantity);
                                    double totalScore = 0;
                                    foreach (var quiz in assessment.Quizzes)
                                    {
                                        var submit = quiz.Scores.FirstOrDefault();
                                        var score = submit?.ScoreNumber ?? 0;
                                        var finalModel = new SingleFinalViewModel()
                                        {
                                            Score = score,
                                            Title = quiz.Title,
                                            Weight = weightPerFinal
                                        };
                                        finals.Add(finalModel);
                                        totalScore += score;
                                    }
                                    avgFinalScore = totalScore / assessment.Quizzes.Count;
                                }
                                var assessmentFinalViewModel = new AssessmentFinalViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    Score = avgFinalScore,
                                    Finals = finals,
                                };
                                assessmentStudentViewModel.AssessmentFinalViewModel = assessmentFinalViewModel;
                                break;
                            }
                        case AssessmentTypeEnum.Attendance:
                            {
                                var totalDaysAttended = classSchedules.Count();
                                var attendedDays = (await _unitOfWork.AttendanceRepository.GetStudentPresentAttendanceByClassIdAsync(classId, studentId)).Count();
                                var score = 0;
                                if (totalDaysAttended != 0) score = (attendedDays / totalDaysAttended) * 10;

                                var assessmentAttendanceViewModel = new AssessmentAttendanceViewModel()
                                {
                                    Id = assessment.Id,
                                    Type = assessment.Type,
                                    CompletionCriteria = assessment.CompletionCriteria ?? 0,
                                    Weight = assessment.Weight ?? 0,
                                    Content = assessment.Content,
                                    Quantity = assessment.Quantity,
                                    AttendedDays = attendedDays,
                                    TotalDaysAttended = totalDaysAttended,
                                    Score = score
                                };
                                assessmentStudentViewModel.AssessmentAttendanceView = assessmentAttendanceViewModel;
                                break;
                            }
                    }

                }
                double totalWeightedScore = 0;
                double totalWeight = 0;

                if (assessmentStudentViewModel.AssessmentAssignmentView != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentAssignmentView.Score * assessmentStudentViewModel.AssessmentAssignmentView.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentAssignmentView.Weight;
                }

                if (assessmentStudentViewModel.AssessmentQuizView != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentQuizView.Score * assessmentStudentViewModel.AssessmentQuizView.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentQuizView.Weight;
                }

                if (assessmentStudentViewModel.AssessmentFinalViewModel != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentFinalViewModel.Score * assessmentStudentViewModel.AssessmentFinalViewModel.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentFinalViewModel.Weight;
                }

                if (assessmentStudentViewModel.AssessmentAttendanceView != null)
                {
                    totalWeightedScore += (assessmentStudentViewModel.AssessmentAttendanceView.Score * assessmentStudentViewModel.AssessmentAttendanceView.Weight);
                    totalWeight += assessmentStudentViewModel.AssessmentAttendanceView.Weight;
                }
                assessmentStudentViewModel.AverageScore = totalWeight > 0 ? totalWeightedScore / totalWeight : 0;
                var minAvgMarkToPass = course?.Syllabus?.MinAvgMarkToPass ?? 0;
                assessmentStudentViewModel.MinAvgMarkToPass = minAvgMarkToPass;

                var lastClassSchedule = classSchedules.OrderByDescending(c => c.Date).FirstOrDefault();
                //thời gian hoàn thành khóa học
                assessmentStudentViewModel.TimeCompleted = (DateTime)lastClassSchedule.Date;

                response = ResponseHandler.Success(assessmentStudentViewModel);

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<AssessmentStudentViewModel>(ex.Message);
            }
            return response;
        }
    }
}
