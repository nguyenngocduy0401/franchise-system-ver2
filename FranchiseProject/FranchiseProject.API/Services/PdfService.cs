using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using iTextSharp.text.pdf;
using System.IO;
using System.Net.Http;

namespace FranchiseProject.API.Services
{
    public class PdfService : IPdfService
    {
        private readonly HttpClient _httpClient;
        public PdfService()
        {
            _httpClient = new HttpClient();
        }
        public async Task<Stream> FillPdfTemplate(CreateContractViewModel contract)
        {
            string firebaseUrl = "https://firebasestorage.googleapis.com/v0/b/franchise-project-1ea45.firebasestorage.app/o/Contract%2Fhop-dong-nhuong-quyen-thuong-mai_1010092534%20(3).pdf?alt=media&token=38cd36c7-a187-4716-891b-52a30132dc16";
            Stream pdfTemplateStream = await DownloadFileFromFirebaseAsync(firebaseUrl);
           

            var StartTime = DateTime.Now;
         //   var EndTime = StartTime.AddYears(contract.Duration);
            var outputStream = new MemoryStream();
            using (var reader = new PdfReader(pdfTemplateStream))
            {
                using (var stamper = new PdfStamper(reader, outputStream))
                {
                   var form = stamper.AcroFields;
                    form.SetField("StartTime", StartTime.ToString("dd/MM/yyyy"));
                 //   form.SetField("EndTime", EndTime.ToString("dd/MM/yyyy"));
                  
                    //form.SetField("Duration", contract.Duration.ToString());
                    //form.SetField("Description", contract.Description ?? "");
                 
                    stamper.FormFlattening = true;
                }
            }
            var finalOutputStream = new MemoryStream(outputStream.ToArray());
            finalOutputStream.Position = 0;
            return finalOutputStream;
        }
/*
        public async Task<Stream> FillUpdatePdfTemplate(UpdateContractViewModel contract)
        {

            string firebaseUrl = "https://firebasestorage.googleapis.com/v0/b/franchise-project-1ea45.firebasestorage.app/o/Contract%2Fhop-dong-nhuong-quyen-thuong-mai_1010092534%20(3).pdf?alt=media&token=38cd36c7-a187-4716-891b-52a30132dc16";
            Stream pdfTemplateStream = await DownloadFileFromFirebaseAsync(firebaseUrl);

           *//* var StartTime = DateTime.Now;
            var EndTime = StartTime.AddYears(contract.Duration);
            var outputStream = new MemoryStream();
            using (var reader = new PdfReader(pdfTemplateStream))
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
            }*//*
            var finalOutputStream = new MemoryStream(outputStream.ToArray());
            finalOutputStream.Position = 0;
            return finalOutputStream;
        }*/
        private async Task<Stream> DownloadFileFromFirebaseAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Kiểm tra xem phản hồi có thành công không.

            var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Đặt lại vị trí của stream về đầu.
            return memoryStream;
        }
    }
}