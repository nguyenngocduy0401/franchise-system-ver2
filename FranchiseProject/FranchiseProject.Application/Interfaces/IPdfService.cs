using FranchiseProject.Application.ViewModels.ContractViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IPdfService
    {
        Task<Stream> FillPdfTemplate(CreateContractViewModel contract);
        Task<Stream> FillUpdatePdfTemplate(UpdateContractViewModel contract);
    }
}
