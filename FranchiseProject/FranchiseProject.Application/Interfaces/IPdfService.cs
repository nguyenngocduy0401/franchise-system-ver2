using FranchiseProject.Application.ViewModels.ContractViewModels;

namespace FranchiseProject.Application.Interfaces
{
    public interface IPdfService
    {
        Task<Stream> FillDocumentTemplate(InputContractViewModel contract);
        Task<Stream> FillPdfTemplate(string studentName, DateTime date, string courseName);
    }
}
