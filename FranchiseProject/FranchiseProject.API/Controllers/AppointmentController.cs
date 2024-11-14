using AutoMapper;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        
        
    }
}
