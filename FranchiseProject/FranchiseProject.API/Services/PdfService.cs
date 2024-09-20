using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModel;
using iTextSharp.text.pdf;
using System.IO;

namespace FranchiseProject.API.Services
{
    public class PdfService : IPdfService
    {
        public Stream FillPdfTemplate(CreateContractViewModel contract)
        {
            string pdfTemplatePath = "D:\\FPT\\franchise-system\\FranchiseProject\\FranchiseProject.Infrastructures\\FireBase\\Resources\\testContract.pdf";
            if (!File.Exists(pdfTemplatePath))
            {
                throw new FileNotFoundException("Mẫu PDF không tồn tại tại đường dẫn: " + pdfTemplatePath);
            }

            var StartTime = DateTime.Now;
            var EndTime = StartTime.AddYears(contract.Duration);
            var outputStream = new MemoryStream();
            using (var reader = new PdfReader(pdfTemplatePath))
            {
                using (var stamper = new PdfStamper(reader, outputStream))
                {
                   var form = stamper.AcroFields;
                    form.SetField("StartTime", StartTime.ToString("dd/MM/yyyy"));
                    form.SetField("EndTime", EndTime.ToString("dd/MM/yyyy"));
                    form.SetField("Amount", contract.Amount.ToString());
                    form.SetField("Duration", contract.Duration.ToString());
                    form.SetField("Description", contract.Description ?? "");
                    form.SetField("TermsAndCondition", contract.TermsAndCondition ?? "");
                    stamper.FormFlattening = true;
                }
            }
            var finalOutputStream = new MemoryStream(outputStream.ToArray());
            finalOutputStream.Position = 0;
            return finalOutputStream;
        }

        public Stream FillUpdatePdfTemplate(UpdateContractViewModel contract)
        {
            string pdfTemplatePath = "D:\\FPT\\franchise-system\\FranchiseProject\\FranchiseProject.Infrastructures\\FireBase\\Resources\\testContract.pdf";
            if (!File.Exists(pdfTemplatePath))
            {
                throw new FileNotFoundException("Mẫu PDF không tồn tại tại đường dẫn: " + pdfTemplatePath);
            }

            var StartTime = DateTime.Now;
            var EndTime = StartTime.AddYears(contract.Duration);
            var outputStream = new MemoryStream();
            using (var reader = new PdfReader(pdfTemplatePath))
            {
                using (var stamper = new PdfStamper(reader, outputStream))
                {
                    var form = stamper.AcroFields;
                    form.SetField("StartTime", StartTime.ToString("dd/MM/yyyy"));
                    form.SetField("EndTime", EndTime.ToString("dd/MM/yyyy"));
                    form.SetField("Amount", contract.Amount.ToString());
                    form.SetField("Duration", contract.Duration.ToString());
                    form.SetField("Description", contract.Description ?? "");
                    form.SetField("TermsAndCondition", contract.TermsAndCondition ?? "");
                    stamper.FormFlattening = true;
                }
            }
            var finalOutputStream = new MemoryStream(outputStream.ToArray());
            finalOutputStream.Position = 0;
            return finalOutputStream;
        }
    }
}