using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;

        public QuizService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //public Task<ApiResponse<bool>>
    }
}
