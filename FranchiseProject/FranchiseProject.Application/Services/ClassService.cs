using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
    
        public ClassService(IMapper mapper, IUnitOfWork unitOfWork,IClaimsService claimsService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
       
        }

        public async Task<ApiResponse<bool>> CreateClassAsync(CreateClassViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (model.Capacity <= 0)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Số lượng học sinh trong 1 lớp phải lớn hơn 0";
                    return response;
                }
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Tên không được để trống!";
                    return response;
                }
                var nameExists = await _unitOfWork.ClassRepository.CheckNameExistAsync(model.Name);
                if (nameExists)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Tên agency đã tồn tại. Không thể tạo mới.";
                    return response;
                }
                var term = await _unitOfWork.TermRepository.GetByIdAsync(Guid.Parse(model.TermId));
                if (term == null) {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Học kỳ không tồn tại. Không thể tạo mới.";
                    return response;
                }

               var user = _claimsService.GetCurrentUserId.ToString();
                var agency = _unitOfWork.UserRepository.GetAgencyIdByUserIdAsync(user);
                if (agency == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Tài Khoản của bạn không thuộc đối tác nào !";
                    return response;
                }
                var course = await _unitOfWork.CourseRepository.GetByIdAsync(Guid.Parse(model.CourseId));
                if (course == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Khóa học không tồn tại. Không thể tạo mới.";
                    return response;
                }
            //    var existingClass = await _unitOfWork.ClassRepository.GetFirstOrDefaultAsync(c =>
            //c.Room == model.RoomName && c.SlotId == model. && c.TermId == model.TermId);
            //    if (existingClass != null)
            //    {
            //        response.Data = false;
            //        response.isSuccess = true;
            //        response.Message = "Lớp học đã tồn tại trong cùng một phòng, slot và học kỳ.";
            //        return response;
            //    }
                var class1 = _mapper.Map<Class>(model);
                class1.AgencyId = agency.Result;
                class1.Status = ClassStatusEnum.Inactive;
                await _unitOfWork.ClassRepository.AddAsync(class1);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo lớp học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<ClassViewModel>>> FilterClassAsync(FilterClassViewModel filter)
        {
            var response = new ApiResponse<Pagination<ClassViewModel>>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                Guid? termId = string.IsNullOrEmpty(filter.TermId) ? (Guid?)null : Guid.Parse(filter.TermId);
                Guid? courseId = string.IsNullOrEmpty(filter.CourseId) ? (Guid?)null : Guid.Parse(filter.CourseId);

                var agencyId = await _unitOfWork.UserRepository.GetAgencyIdByUserIdAsync(userId);
                if (!agencyId.HasValue)
                {
                    response.Data = null;
                    response.isSuccess = false;
                    response.Message = "Người dùng không thuộc về bất kỳ agency nào.";
                    return response;
                }
                Expression<Func<Class, bool>> filterExpression = c =>
                    (string.IsNullOrEmpty(filter.Name) || c.Name.Contains(filter.Name)) &&
                    (!filter.Status.HasValue || c.Status == filter.Status) &&
                     (filter.TermId == null || c.TermId == termId) && 
            (filter.CourseId == null || c.CourseId == courseId) && 
                    (c.AgencyId == agencyId)&&
                    (!filter.IsDeleted.HasValue || c.IsDeleted == filter.IsDeleted);

                var classes = await _unitOfWork.ClassRepository.GetFilterAsync(
                    filter: filterExpression,
                    includeProperties: "Course,Term",
                    pageIndex: filter.PageIndex,
                    pageSize: filter.PageSize
                );

                var classViewModels = _mapper.Map<Pagination<ClassViewModel>>(classes);
                foreach (var classViewModel in classViewModels.Items)
                {
                    var enrollCount = await _unitOfWork.StudentClassRepository.CountStudentsByClassIdAsync(classViewModel.Id);
                    classViewModel.CurrentEnrollment = enrollCount; 
                }
                response.Data = classViewModels;
                response.isSuccess = true;
                response.Message = "Lấy danh sách lớp thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<ClassViewModel>> GetClassByIdAsync(string id)
        {
            var response = new ApiResponse<ClassViewModel>();
            try
            {
                var class1 = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(id));
                if (class1 == null)
                {
                    response.Data = null;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy lớp !";
                    return response;
                }
                var term = await _unitOfWork.TermRepository.GetByIdAsync(class1.TermId.Value);
                var course = await _unitOfWork.CourseRepository.GetByIdAsync(class1.CourseId.Value);
                var clasViewModel = _mapper.Map<ClassViewModel>(class1);
                clasViewModel.CourseName = course.Name;
                clasViewModel.TermName= term.Name;
                var enrollcount = await _unitOfWork.StudentClassRepository.CountStudentsByClassIdAsync(Guid.Parse(id));
                clasViewModel.CurrentEnrollment = enrollcount;
                response.Data = clasViewModel;
                response.isSuccess = true;
                response.Message = "tìm slot học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<ClassStudentViewModel>>> GetListStudentInClassAsync(string id)
        {
            var response = new ApiResponse<Pagination<ClassStudentViewModel>>();
            try
            {
              

                var classEntity = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(id));
                if (classEntity == null)
                {
                    response.Data = null;
                    response.isSuccess = false;
                    response.Message = "Không tìm thấy lớp học với ID này";
                    return response;
                }

             
                var studentsInClass = await _unitOfWork.StudentClassRepository.GetFilterAsync(s => s.ClassId == classEntity.Id);
                var classStudentViewModel = new ClassStudentViewModel
                {
                    ClassName = classEntity.Name,

                    StudentInfo = studentsInClass.Select(s => new StudentClassViewModel
                    {
                        StudentName = s.User.FullName,
                        DateOfBirth = s.User.DateOfBirth,
                        URLImage = s.User.URLImage
                    }).ToList()
                };

          
                var paginationResult = new Pagination<ClassStudentViewModel>
                {
                    Items = new List<ClassStudentViewModel> { classStudentViewModel },
                    PageIndex = 1,  
                    PageSize = 1,
                    TotalItemsCount = 1
                };

            
                response.Data = paginationResult;
                response.isSuccess = true;
                response.Message = "Lấy danh sách học sinh thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> UpdateClassAsync(CreateClassViewModel update,string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (update.Capacity <= 0)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Số lượng học sinh trong 1 lớp phải lớn hơn 0";
                    return response;
                }
                if (string.IsNullOrWhiteSpace(update.Name))
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Tên không được để trống!";
                    return response;
                }
                var nameExists = await _unitOfWork.ClassRepository.CheckNameExistAsync(update.Name);
                if (nameExists)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Tên agency đã tồn tại. Không thể tạo mới.";
                    return response;
                }
                var term = await _unitOfWork.TermRepository.GetByIdAsync(Guid.Parse(update.TermId));
                if (term == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Học kỳ không tồn tại. Không thể tạo mới.";
                    return response;
                }
                var course = await _unitOfWork.CourseRepository.GetByIdAsync(Guid.Parse(update.CourseId));
                if (course == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Khóa học không tồn tại. Không thể tạo mới.";
                    return response;
                }
                var user = _claimsService.GetCurrentUserId.ToString();
                var agency = _unitOfWork.UserRepository.GetAgencyIdByUserIdAsync(user);
                if (agency == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Tài Khoản của bạn không thuộc đối tác nào !";
                    return response;
                }
                var class1 = await _unitOfWork.ClassRepository.GetExistByIdAsync(Guid.Parse(id));
                _mapper.Map(update, class1);
                class1.AgencyId = agency.Result;
                _unitOfWork.ClassRepository.Update(class1);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Udpate fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "cập nhật lớp học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateClassStatusAsync(ClassStatusEnum status, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var class1 = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(id));
                if (class1 == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy lớp !";
                    return response;
                }
                class1.Status = status;
                _unitOfWork.ClassRepository.Update(class1);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Udpate fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "cập nhật lớp  học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<StudentClassScheduleViewModel>>> GetClassSchedulesForCurrentUserByTermAsync(string termId,int pageIndex, int pageSize)
        {
            var response = new ApiResponse<Pagination<StudentClassScheduleViewModel>>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                var classSchedules = await _unitOfWork.StudentClassRepository.GetClassSchedulesByUserIdAndTermIdAsync(userId, Guid.Parse(termId));

                if (classSchedules == null || !classSchedules.Any())
                {
                    response.Data = new Pagination<StudentClassScheduleViewModel>
                    {
                        Items = new List<StudentClassScheduleViewModel>(),
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalItemsCount = 0
                    };
                    response.isSuccess = true; 
                    response.Message = "Không có lịch học nào trong kỳ đã chọn.";
                    return response;
                }
                var classScheduleViewModels = new List<StudentClassScheduleViewModel>();
                foreach (var cs in classSchedules)
                {
                    var slot = await _unitOfWork.SlotRepository.GetByIdAsync(cs.SlotId.Value);
                    var classScheduleViewModel = new StudentClassScheduleViewModel
                    {
                          Id = cs.Id != null ? cs.Id.ToString() : Guid.Empty.ToString(),
                        Room = cs.Room,
                        ClassName = cs.Class?.Name,
                        SlotName = slot?.Name,
                        Date = cs.Date?.ToString("dd/MM/yyyy"),
                        StartTime = slot?.StartTime.ToString(),
                        EndTime = slot?.EndTime.ToString(),
                    };

                    classScheduleViewModels.Add(classScheduleViewModel);
                }

                var pagination = new Pagination<StudentClassScheduleViewModel>
                {
                    Items = classScheduleViewModels,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalItemsCount = await _unitOfWork.StudentClassRepository.CountClassSchedulesByUserIdAndTermIdAsync(userId, Guid.Parse(termId))
                };
                response.Data = pagination;
                response.isSuccess = true;
                response.Message = "Lấy lịch học theo kỳ thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteClassAsync(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var class1 = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(id));
                if (class1 == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy lớp học!";
                    return response;
                }
                switch (class1.IsDeleted)
                {
                    case false:
                        _unitOfWork.ClassRepository.SoftRemove(class1);
                        response.Message = "Xoá lớp học thành công!";
                        break;
                    case true:
                        _unitOfWork.ClassRepository.RestoreSoftRemove(class1);
                        response.Message = "Phục hồi lớp học thành công!";
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
        //----- Api lấy các lớp học chưa lịch học 
        public async Task<ApiResponse<Pagination<ClassViewModel>>> GetClassesWithoutScheduleAsync(int pageIndex, int pageSize)
        {
            var response = new ApiResponse<Pagination<ClassViewModel>>();
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                var agencyId = await _unitOfWork.UserRepository.GetAgencyIdByUserIdAsync(userId.ToString());
                if (!agencyId.HasValue)
                {
                    response.Data = null;
                    response.isSuccess = false;
                    response.Message = "Người dùng không thuộc về bất kỳ agency nào.";
                    return response;
                }
                var totalItemsCount = await _unitOfWork.ClassRepository.CountAsync(
             c => c.AgencyId == agencyId && !c.ClassSchedules.Any()
         );
                var classesWithoutSchedule = await _unitOfWork.ClassRepository.GetFilterAsync(
                    filter: c => c.AgencyId == agencyId && !c.ClassSchedules.Any(),
                    includeProperties: "Term,Course",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
                var pagination = _mapper.Map<Pagination<ClassViewModel>>(classesWithoutSchedule);
                pagination.TotalItemsCount = totalItemsCount;

                response.Data = pagination;
                response.isSuccess = true;
                response.Message = "Lấy danh sách lớp không có lịch học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
