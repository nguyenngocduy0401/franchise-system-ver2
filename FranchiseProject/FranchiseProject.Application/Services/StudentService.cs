using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace FranchiseProject.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateStudentViewModel> _validator;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailService _emailService;

        public StudentService(IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateStudentViewModel> validator, IUserService userService, IMapper mapper, IHubContext<NotificationHub> hubContext, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _userService = userService;
            _mapper = mapper;
            _hubContext = hubContext;
            _emailService = emailService;
        }






        /*  public async Task<ApiResponse<bool>> CreateStudentAsync(CreateStudentViewModel createStudentViewModel,string agencyId)
          {
              var response= new ApiResponse<bool>();
              try
              {
                  FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(createStudentViewModel);
                  if (!validationResult.IsValid)
                  {
                      response.isSuccess = false;
                      response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                      return response;
                  }
                  var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(Guid.Parse(agencyId));
                  var student=_mapper.Map<Us>(createStudentViewModel);
                  student.Status=StudentStatusEnum.pending;
                  student.AgencyId=agency.Id;
                  await _unitOfWork.StudentRepository.AddAsync(student);
                  var isSuccess = await _unitOfWork.SaveChangeAsync();
                  if (isSuccess > 0)
                  {
                      response.Data = true;
                      response.isSuccess = true;
                      response.Message = "Tạo Thành Công !";
                      var emailMessage = new MessageModel
                      {
                          To = createStudentViewModel.Email,
                          Subject = "Đăng ký thành công  [futuretech-noreply]",
                          Body = $"<p>Chào {createStudentViewModel.StudentName},</p>" +
                        $"<p>Cảm ơn bạn đã đăng ký khóa học của chúng tôi .</p>" +
                        $"<p>Thông tin của bạn đã được ghi nhận thành công .</p>" +
                         $"<p>Nhân viên trung tâm sẻ liên hệ hổ trợ bạn sớm nhất có thể  .</p>" +
                        $"<p>Trân trọng,</p>" +
                        $"<p>Đội ngũ {agency.Name}</p>"// tên fix 
                      };
                      bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                      if (!emailSent)
                      {
                          response.Message += " (Lỗi khi gửi email)";
                      }
                  }
                  else
                  {
                      throw new Exception("Create unsuccesfully ");
                  }
              }
              catch (DbException ex)
              {
                  response.isSuccess = false;
                  response.Message = ex.Message;

              }
              catch (Exception ex)
              {
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }
              return response;
          }

          public async Task<ApiResponse<bool>> DeleteStudentAsync(string studentId)
          {
              var response = new ApiResponse<bool>();
              try
              {
                  var student = await _unitOfWork.StudentRepository.GetByIdAsync(Guid.Parse(studentId));
                  if (student == null)
                  {
                      response.Data = false;
                      response.isSuccess = true;
                      response.Message = "Không tìm thấy Học sinh!";
                      return response;
                  }
                  switch (student.IsDeleted)
                  {
                      case false:
                          _unitOfWork.StudentRepository.SoftRemove(student);
                          response.Message = "Xoá Học sinh thành công!";
                          break;
                      case true:
                          _unitOfWork.StudentRepository.RestoreSoftRemove(student);
                          response.Message = "Phục hồi Học sinh thành công!";
                          break;
                  }
                  var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                  if (!isSuccess) throw new Exception("Delete fail!");
                  response.Data = true;
                  response.isSuccess = true;
              }
              catch (Exception ex)
              {
                  response.Data = false;
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }
              return response;
          }


          public async Task<ApiResponse<StudentViewModel>> GetStudentByIdAsync(string studentId)
          {
              var response = new ApiResponse<StudentViewModel>();
              try
              {
                  var student = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(studentId));
                  if (student == null)
                  {
                      response.Data = null;
                      response.isSuccess = true;
                      response.Message = "Không tìm thấy Học Sinh!";
                      return response;
                  }
                  var studentViewModel = _mapper.Map<StudentViewModel>(student);
                  response.Data = studentViewModel;
                  response.isSuccess = true;
                  response.Message = "tìm Học Sinh thành công!";
              }
              catch (Exception ex)
              {
                  response.Data = null;
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }
              return response;
          }
          public async Task<ApiResponse<bool>> UpdateStudentAsync(CreateStudentViewModel updateStudentViewModel,string studentId)
          {
              var response = new ApiResponse<bool>();
              try
              {
                  FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(updateStudentViewModel);
                  if (!validationResult.IsValid)
                  {
                      response.isSuccess = false;
                      response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                      return response;
                  }
                  var student = await _unitOfWork.StudentRepository.GetExistByIdAsync(Guid.Parse(studentId));
                  _mapper.Map(updateStudentViewModel, student);
                  _unitOfWork.StudentRepository.Update(student);
                  var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                  if (!isSuccess) throw new Exception("Udpate fail!");
                  response.Data = true;
                  response.isSuccess = true;
                  response.Message = "cập nhật Học sinh  thành công!";

              }
              catch (Exception ex)
              {
                  response.Data = false;
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }
              return response;
          }
          public async Task<ApiResponse<bool>> UpdateStatusStudentAsync(StudentStatusEnum status, string studentId)
          {
              var response = new ApiResponse<bool>();
              try
              {

                  var student = await _unitOfWork.StudentRepository.GetByIdAsync(Guid.Parse(studentId));
                  if (student == null)
                  {
                      response.Data = false;
                      response.isSuccess = true;
                      response.Message = "Không tìm thấy học sinh ";
                      return response;
                  }
                  if (student.Status == status)
                  {
                      response.Data = false;
                      response.isSuccess = false;
                      response.Message = $"Trạng thái đã là {status}, không thể cập nhật lại.";
                      return response;
                  }
                  switch (status)
                  {
                      case StudentStatusEnum.Waitlisted:
                          var existingUser = await _unitOfWork.UserRepository.GetByAgencyIdAsync(Guid.Parse(studentId));
                          if (existingUser == null)
                          {
                              var userCredentials = await _userService.GenerateUserCredentials(student.StudentName);

                              var newUser = new User
                              {
                                  UserName = userCredentials.UserName,
                                  PasswordHash = userCredentials.Password,
                                  AgencyId = student.AgencyId,
                                  Status = UserStatusEnum.active,
                                  Email = student.Email,
                                  PhoneNumber = student.PhoneNumber,


                              };
                              var user2 = _unitOfWork.UserRepository.CreateUserAndAssignRoleAsync(newUser, RolesEnum.AgencyManager.ToString());
                              var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(student.AgencyId.Value);
                              var emailMessage = new MessageModel
                              {
                                  To = student.Email,
                                  Subject = " Chào Mừng Học Viên mới [noreply]!!!",
                                  Body = $"<p>Chào {student.StudentName},</p>" +
                                     $"<p>Chúng tôi rất vui mừng khi bạn lựa chọn {agency.Name}!</p>" +
                                      $"<p>Đây là tài khoản để bạn có thể đăng nhập hệ thống:</p>" +
                                     $"<ul>" +
                                     $"<li><strong>Username:</strong> {newUser.UserName}</li>" +
                                     $"<li><strong>Password:</strong> {userCredentials.Password}</li>" +
                                     $"</ul>" +
                                     $"<p>Vui lòng bảo mật thông tin đăng nhập này.</p>" +
                                     $"<p>Chúc mừng và hẹn gặp lại!</p>" +
                                     $"<p>Trân trọng,</p>" +
                                     $"<p>Đội ngũ {agency.Name}</p>"
                              };
                              bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                              if (!emailSent)
                              {
                                  response.Message += " (Lỗi khi gửi email)";
                              }
                          }

                          else
                          {
                              existingUser.Status = UserStatusEnum.active;
                          }
                          break;
                      case StudentStatusEnum.Enrolled:
                          var studentregister = await _unitOfWork.StudentRepository.GetByIdAsync(Guid.Parse(studentId));
                          if (studentregister != null && studentregister.StatusPayment == StudentPaymentStatusEnum.Completed)
                          {
                              var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(student.AgencyId.Value);
                              var emailMessage = new MessageModel
                              {
                                  To = studentregister.Email,
                                  Subject = "Đăng ký thành công  [noreply]",
                                  Body = $"<p>Chào {studentregister.StudentName},</p>" +
                                   $"<p>Bạn đã enroll vào lớp học thành công </p>" +
                                    $"<p>Hãy truy cập vào website để kiểm tra lịch học </p>" +
                                   $"<p>Trân trọng,</p>" +
                                   $"<p>Đội ngũ {agency.Name}</p>"
                              };
                              bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                              if (!emailSent)
                              {
                                  response.Message += " (Lỗi khi gửi email)";
                              }
                          }
                          else 
                          {
                              response.Data = false;
                              response.isSuccess = true;
                              response.Message = "Học sinh không tồn tại hoặc chưa thanh toán thành công !";
                              return response;
                          }

                          break;




                  }
                  student.Status = status;
                  var isSuccess = await _unitOfWork.SaveChangeAsync();
                  if (isSuccess > 0)
                  {
                      response.Data = true;
                      response.isSuccess = true;
                      response.Message = "Cập nhật trạng thái Đối tác thành công!";
                  }
                  else
                  {
                      throw new Exception("Failed to update agency status.");
                  }
              }
              catch (DbException ex)
              {
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }
              catch (Exception ex)
              {
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }

              return response;
          }
          public async Task<ApiResponse<bool>> UpdatStudentPaymentStatusAsync(StudentPaymentStatusEnum status, string studentId)
          {
              var response = new ApiResponse<bool>();
              try
              {

                  var student = await _unitOfWork.StudentRepository.GetByIdAsync(Guid.Parse(studentId));
                  if (student == null)
                  {
                      response.Data = false;
                      response.isSuccess = true;
                      response.Message = "Không tìm thấy học sinh ";
                      return response;
                  }
                  if (student.StatusPayment == status)
                  {
                      response.Data = false;
                      response.isSuccess = false;
                      response.Message = $"Trạng thái đã là {status}, không thể cập nhật lại.";
                      return response;
                  }
                  switch (status)
                  {
                      case StudentPaymentStatusEnum.Refunded:
                          var student1 = await _unitOfWork.StudentRepository.GetByIdAsync(Guid.Parse(studentId));
                          if (student1 != null && student1.StatusPayment==StudentPaymentStatusEnum.Completed)
                          { 

                              var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(student.AgencyId.Value);
                            //  var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(existingUser);
                              var emailMessage = new MessageModel
                              {
                                  To = student.Email,
                                  Subject = " Thông báo đẫ hoàn tiền [noreply]",
                                  Body = $"<p>Chào {student.StudentName},</p>" +
                                     $"<p>Chúng tôi rất vui mừng khi bạn lựa chọn {agency.Name}!</p>" +
                                      $"<p>Tiền Đã được hoàn lại .</p>" +
                                      $"<p>Bạn có thể liên hệ hotline để được hổ trợ thêm !</p>" +
                                     $"<p>Trân trọng,</p>" +
                                     $"<p>Đội ngũ {agency.Name}</p>"
                              };
                              bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                              if (!emailSent)
                              {
                                  response.Message += " (Lỗi khi gửi email)";
                              }
                          }

                          else
                          {
                              response.Data = false;
                              response.isSuccess = true;
                              response.Message = "Học sinh không tồn tại hoặc chưa thanh toán thành công !";
                              return response;
                          }
                          break;
                      case StudentPaymentStatusEnum.Pending_Payment:
                          var studentregister = await _unitOfWork.StudentRepository.GetByIdAsync(Guid.Parse(studentId));
                          if (studentregister != null && studentregister.StatusPayment == StudentPaymentStatusEnum.Completed)
                          {
                              var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(student.AgencyId.Value);
                              var emailMessage = new MessageModel
                              {
                                  To = studentregister.Email,
                                  Subject = "Đăng ký thành công  [noreply]",
                                  Body = $"<p>Chào {studentregister.StudentName},</p>" +
                                   $"<p>Bạn đã enroll vào lớp học thành công </p>" +
                                    $"<p>Hãy truy cập vào website để kiểm tra lịch học </p>" +
                                   $"<p>Trân trọng,</p>" +
                                   $"<p>Đội ngũ {agency.Name}</p>"
                              };
                              bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                              if (!emailSent)
                              {
                                  response.Message += " (Lỗi khi gửi email)";
                              }
                          }
                          else
                          {
                              response.Data = false;
                              response.isSuccess = true;
                              response.Message = "Học sinh không tồn tại hoặc chưa thanh toán thành công !";
                              return response;
                          }

                          break;




                  }
                  student.Status = status;
                  var isSuccess = await _unitOfWork.SaveChangeAsync();
                  if (isSuccess > 0)
                  {
                      response.Data = true;
                      response.isSuccess = true;
                      response.Message = "Cập nhật trạng thái Đối tác thành công!";
                  }
                  else
                  {
                      throw new Exception("Failed to update agency status.");
                  }
              }
              catch (DbException ex)
              {
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }
              catch (Exception ex)
              {
                  response.isSuccess = false;
                  response.Message = ex.Message;
              }

              return response;
          }
          public Task<ApiResponse<bool>> StudentEnrollClassAsync(string classId)
          {
              throw new NotImplementedException();
          }





          public Task<ApiResponse<Pagination<StudentViewModel>>> FilterStudentAsync(FilterStudentViewModel filter)
          {
              throw new NotImplementedException();
          }
          public Task<ApiResponse<Pagination<ClassForStudentViewModel>>> GetClassForStudentAsync(string studentId)
          {
              throw new NotImplementedException();
          }

          public Task<ApiResponse<int>> CountStudenInCourseAsync()
          {
              throw new NotImplementedException();
          }
      }
  }*/
    }
}