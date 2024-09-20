using AutoMapper;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
<<<<<<< HEAD
using FranchiseProject.Application.ViewModels.ContractViewModel;
=======
using FranchiseProject.Application.ViewModels.UserViewModels;
>>>>>>> fe3ee3b3bca4e0caa1da32b242e99a2c4327a23a
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Mappers
{
    public class MapperConfigurationsProfile : Profile 
    {
        public MapperConfigurationsProfile() {
<<<<<<< HEAD
            #region FranchiseRegist
            CreateMap<RegisterFranchiseViewModel, FranchiseRegistrationRequests>();

            CreateMap<FranchiseRegistrationRequests, RegisterFranchiseViewModel>().ForMember(dest => dest.CustomerName,otp=>otp.MapFrom(src => src.CusomterName)).ReverseMap();
            CreateMap<FranchiseRegistrationRequests, FranchiseRegistrationRequestsViewModel>()
           .ForMember(dest => dest.ConsultantUserName, opt => opt.MapFrom(src => src.User.UserName)).ReverseMap();
            #endregion
            #region Agency
            CreateMap<CreateAgencyViewModel, Agency>().ReverseMap();
            #endregion
            #region Contract
            CreateMap<CreateContractViewModel, Contract>().ReverseMap();
=======

            CreateMap<RegisFranchiseViewModel, FranchiseRegistrationRequests>();
            CreateMap<FranchiseRegistrationRequests, RegisFranchiseViewModel>().ForMember(dest => dest.CustomerName,otp=>otp.MapFrom(src => src.CusomterName)).ReverseMap();
            CreateMap<FranchiseRegistrationRequests, ConsultationViewModel>()
           .ForMember(dest => dest.ConsultantUserName, opt => opt.MapFrom(src => src.User.UserName)).ReverseMap();

            #region User
            CreateMap<User, UserViewModel>();
>>>>>>> fe3ee3b3bca4e0caa1da32b242e99a2c4327a23a
            #endregion
        }
    }
}
