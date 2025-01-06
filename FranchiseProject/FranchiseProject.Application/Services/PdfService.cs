using DocumentFormat.OpenXml.Wordprocessing;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using iText.Forms;
using iText.IO.Font;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using Xceed.Words.NET;

namespace FranchiseProject.API.Services
{
    public class PdfService : IPdfService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public PdfService(IConfiguration configuration)
        {
            _httpClient = new HttpClient(); _configuration = configuration;
        }


        public async Task<Stream> FillPdfTemplate(string studentName, DateTime date, string courseName)
        {
            string firebaseUrl = "https://firebasestorage.googleapis.com/v0/b/franchise-project-1ea45.firebasestorage.app/o/net_intern_NguyenNgocDuy_resume.pdf?alt=media&token=1dfcbc6d-6bad-4909-9ad1-322c79aef27d";
            Stream pdfTemplateStream = await DownloadFileFromFirebaseAsync(firebaseUrl);

            if (pdfTemplateStream == null)
            {
                throw new Exception("Không thể tải tệp PDF từ Firebase.");
            }

            var outputStream = new MemoryStream();
            try
            {
                using (var reader = new PdfReader(pdfTemplateStream))
                {
                    if (reader == null)
                    {
                        throw new Exception("Không thể tạo PdfReader từ stream.");
                    }
                    using (var stamper = new PdfStamper(reader, outputStream))
                    {
                        // Kiểm tra đường dẫn font
                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
                        if (!File.Exists(fontPath))
                        {
                            throw new FileNotFoundException($"Font ARIAL.TTF không tìm thấy tại {fontPath}");
                        }

                        BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        float fontSize = 12.50f;
                        var form = stamper.AcroFields;

                        // In ra tất cả các tên trường trong PDF để xác nhận
                        foreach (var field in form.Fields)
                        {
                            Console.WriteLine($"Field name: {field.Key}");
                        }

                        // Hàm điền vào trường với cỡ chữ và font
                        void SetFieldWithFontSize(string fieldName, string value)
                        {
                            if (form.Fields.ContainsKey(fieldName))
                            {
                                form.SetFieldProperty(fieldName, "textfont", bf, null);
                                form.SetFieldProperty(fieldName, "textsize", fontSize, null);
                                form.SetFieldProperty(fieldName, "alignment", PdfFormField.Q_CENTER, null);
                                form.SetField(fieldName, value);
                            }
                            else
                            {
                                Console.WriteLine($"Trường {fieldName} không tồn tại trong PDF.");
                            }
                        }

                        // Điền thông tin vào các trường
                        SetFieldWithFontSize("ContractCode", studentName ?? "");
                        SetFieldWithFontSize("Date", date.ToString("yyyy-MM-dd"));
                        SetFieldWithFontSize("CourseName", courseName ?? "");

                        stamper.FormFlattening = true;
                    }
                }

                // Trả về stream kết quả
                var finalOutputStream = new MemoryStream(outputStream.ToArray());
                finalOutputStream.Position = 0;
                return finalOutputStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
                throw;
            }
        }

      
        public async Task<Stream> FillDocumentTemplate(InputContractViewModel contract)
        {
            string templatePath = _configuration["ContractTemplateUrl"];

            using (var doc = DocX.Load(templatePath))
            {
                var Deposit = contract.Deposit;
                var totalMoneyParse = contract.TotalMoney.HasValue ? NumberToWordsConverter.ConvertToWords(contract.TotalMoney.Value) : "";
                var depositParse = Deposit.HasValue ? NumberToWordsConverter.ConvertToWords(Deposit.Value) : "";
                var FranchiseFeeParse = contract.FranchiseFee.HasValue ? NumberToWordsConverter.ConvertToWords(contract.FranchiseFee.Value) : "";
                doc.ReplaceText("{{TotalMoney}}", contract.TotalMoney?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{Deposit}}", Deposit?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{FranchiseFee}}", contract.FranchiseFee?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{TotalMoneyParse}}", totalMoneyParse);
                doc.ReplaceText("{{DepositParse}}", depositParse);
                doc.ReplaceText("{{FranchiseFeeParse}}", FranchiseFeeParse);
                doc.ReplaceText("{{ContractCode}}", contract.ContractCode ?? "");
                doc.ReplaceText("{{Duration}}", contract.Druration?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{Percent}}", contract.Percent?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{Address}}", contract.Address ?? "");

                var memoryStream = new MemoryStream();
                doc.SaveAs(memoryStream);
                memoryStream.Position = 0;

                return memoryStream;
            }
        }
        private async Task<Stream> DownloadFileFromFirebaseAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Kiểm tra thành công

                var memoryStream = new MemoryStream();
                await response.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Đặt lại vị trí của stream về đầu
                return memoryStream;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tải tệp, ví dụ: không tìm thấy URL, không kết nối được với Firebase
                throw new Exception($"Lỗi khi tải tệp từ Firebase: {ex.Message}", ex);
            }
        }
        public class NumberToWordsConverter
        {
            private static readonly string[] Units = { "", "nghìn", "triệu", "tỷ", "nghìn tỷ", "triệu tỷ" };
            private static readonly string[] Numbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

            public static string ConvertToWords(double number)
            {
                if (number == 0) return "không đồng";

                var words = new List<string>();
                var numStr = ((long)number).ToString("000000000000");
                var groups = new string[4];

                for (int i = 0; i < 4; i++)
                {
                    groups[i] = numStr.Substring(i * 3, 3);
                }

                for (int i = 0; i < 4; i++)
                {
                    var groupNum = int.Parse(groups[i]);
                    if (groupNum != 0)
                    {
                        words.Add(ConvertGroupToWords(groupNum));
                        if (i < 3) words.Add(Units[3 - i]);
                    }
                }

                var result = string.Join(" ", words).Trim();
                result = result.Replace("một mươi", "mười");
                result = result.Replace("mười năm", "mười lăm");
                result = result.Replace("mươi năm", "mươi lăm");
                return result ;
            }

            private static string ConvertGroupToWords(int number)
            {
                var words = new List<string>();
                var hundreds = number / 100;
                var remainder = number % 100;
                var tens = remainder / 10;
                var ones = remainder % 10;

                if (hundreds > 0)
                {
                    words.Add(Numbers[hundreds] + " trăm");
                    if (remainder > 0 && remainder < 10)
                    {
                        words.Add("lẻ");
                    }
                }

                if (tens > 1)
                {
                    words.Add(Numbers[tens] + " mươi");
                    if (ones == 1)
                    {
                        words.Add("mốt");
                    }
                    else if (ones > 0)
                    {
                        words.Add(Numbers[ones]);
                    }
                }
                else if (tens == 1)
                {
                    words.Add("mười");
                    if (ones > 0)
                    {
                        words.Add(Numbers[ones]);
                    }
                }
                else if (ones > 0)
                {
                    words.Add(Numbers[ones]);
                }

                return string.Join(" ", words).Trim();
            }
        }
    }
}