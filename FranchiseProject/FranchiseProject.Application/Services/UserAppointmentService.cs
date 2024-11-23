using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class UserAppointmentService : IUserAppointmentService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentService _appointmentService;
        public UserAppointmentService(IMapper mapper, IUnitOfWork unitOfWork,
            IAppointmentService appointmentService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
        }
        public async Task<ApiResponse<bool>> CreateUserAppointmentAsync(Guid appointmentId, List<string> userIds)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var appointment = (await _unitOfWork.AppointmentRepository.FindAsync(e => e.Id == appointmentId)).FirstOrDefault();
                var checkAppointmentAvailable = _appointmentService.CheckAppointmentAvailable(appointment);
                if (checkAppointmentAvailable.Data == false) return checkAppointmentAvailable;

                var oldUserAppointment = (await _unitOfWork.UserAppointmentRepository
                    .FindAsync(e => e.AppointmentId == appointmentId)).ToList();

                _unitOfWork.UserAppointmentRepository.HardRemoveRange(oldUserAppointment);

                var listUserAppointment = new List<UserAppointment>();
                foreach(var userId in userIds)
                {
                    listUserAppointment.Add(new UserAppointment 
                        { 
                            AppointmentId = appointmentId,
                            UserId = userId,
                        });
                }
                await _unitOfWork.UserAppointmentRepository.AddRangeAsync(listUserAppointment);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Sửa danh sách người tham gia thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}
