using FranchiseProject.Application.ViewModels.ContractViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IPdfService
    {
        Stream FillPdfTemplate(CreateContractViewModel contract);
        Stream FillUpdatePdfTemplate(UpdateContractViewModel contract);
    }
}
