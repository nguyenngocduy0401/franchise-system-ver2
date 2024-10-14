using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                var class1 = _mapper.Map<Class>(model);
                class1.AgencyId = agency.Result;
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
                Expression<Func<Class, bool>> filterExpression = c =>
                    (string.IsNullOrEmpty(filter.Name) || c.Name.Contains(filter.Name)) &&
                    (!filter.Status.HasValue || c.Status == filter.Status) &&
                    (filter.TermName == null || !filter.TermName.Any() || filter.TermName.Contains(c.Term.Name)) &&
                    (filter.CourseName == null || !filter.CourseName.Any() || filter.CourseName.Contains(c.Course.Name));

                var classes = await _unitOfWork.ClassRepository.GetFilterAsync(
                    filter: filterExpression,
                    includeProperties: "Course,Term",
                    pageIndex: filter.PageIndex,
                    pageSize: filter.PageSize
                );

                var classViewModels = _mapper.Map<Pagination<ClassViewModel>>(classes);
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
    }
}
