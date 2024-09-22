using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.Application.ViewModels.ContractViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
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
            #region FranchiseRegister
            CreateMap<ConsultationViewModel, FranchiseRegistrationRequests>().ReverseMap();
            CreateMap<RegisterConsultationViewModel, FranchiseRegistrationRequests>();
            CreateMap<FranchiseRegistrationRequests, ConsultationViewModel>().ForMember(dest => dest.CusomterName,otp=>otp.MapFrom(src => src.CusomterName)).ReverseMap();
            CreateMap<FranchiseRegistrationRequests, ConsultationViewModel>()
           .ForMember(dest => dest.ConsultantUserName, opt => opt.MapFrom(src => src.User.UserName)).ReverseMap();
            #endregion
            #region Agency
            CreateMap<CreateAgencyViewModel, Agency>().ReverseMap();
            CreateMap<Agency, AgencyViewModel>();
            #endregion
            #region Contract
            CreateMap<CreateContractViewModel, Contract>().ReverseMap();
            CreateMap<Contract, ContractViewModel>().ForMember(dest => dest.AgencyName,otp => otp.MapFrom(src=>src.Agency.Name));

            #endregion

          
            #region User
            CreateMap<User, UserViewModel>();
            CreateMap<CreateUserByAdminModel, User>()
            .ForMember(dest => dest.AgencyId, opt => opt
            .MapFrom(src => string.IsNullOrEmpty(src.AgencyId) ? (Guid?)null : Guid.Parse(src.AgencyId)))
            .ForMember(dest => dest.ContractId, opt => opt
            .MapFrom(src => string.IsNullOrEmpty(src.ContractId) ? (Guid?)null : Guid.Parse(src.ContractId)))
            .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password)); 
            #endregion

            #region Slot
            CreateMap<CreateSlotModel, Slot>();
            CreateMap<Slot, SlotViewModel>();
            CreateMap<Pagination<Slot>, Pagination<SlotViewModel>>().ReverseMap();
            #endregion
        }
    }
}
