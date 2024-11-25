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
        public async Task<Stream> FillPdfTemplate(InputContractViewModel contract)
        {
            string firebaseUrl = "https://firebasestorage.googleapis.com/v0/b/franchise-project-1ea45.firebasestorage.app/o/Contract%2Fhop-dong-nhuong-quyen-thuong-mai_1010092534%20(3).pdf?alt=media&token=367beaa1-68f5-4734-9a0f-841d7d17fe99";
            Stream pdfTemplateStream = await DownloadFileFromFirebaseAsync(firebaseUrl);

            var Deposit = contract.TotalMoney * 0.2; // 20% of TotalMoney
            var totalMoneyParse = contract.TotalMoney.HasValue ? NumberToWordsConverter.ConvertToWords(contract.TotalMoney.Value) : "";
            var depositParse = Deposit.HasValue ? NumberToWordsConverter.ConvertToWords(Deposit.Value) : "";
            var designFeeParse = contract.DesignFee.HasValue ? NumberToWordsConverter.ConvertToWords(contract.DesignFee.Value) : "";
            var FranchiseFeeParse = contract.FranchiseFee.HasValue ? NumberToWordsConverter.ConvertToWords(contract.FranchiseFee.Value) : "";

            var outputStream = new MemoryStream();
            using (var reader = new PdfReader(pdfTemplateStream))
            {
                using (var stamper = new PdfStamper(reader, outputStream))
                {
                    var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    float fontSize = 12.50f;
                    var form = stamper.AcroFields;

                    void SetFieldWithFontSize(string fieldName, string value)
                    {
                        form.SetFieldProperty(fieldName, "textfont", bf, null);
                        form.SetFieldProperty(fieldName, "textsize", fontSize, null);
                        form.SetFieldProperty(fieldName, "alignment", PdfFormField.Q_CENTER, null);
                        form.SetField(fieldName, value);
                    }

                    SetFieldWithFontSize("TotalMoney", contract.TotalMoney?.ToString("#,##0") ?? "");
                    SetFieldWithFontSize("Deposit", Deposit?.ToString("#,##0") ?? "");
                    SetFieldWithFontSize("DesignFee", contract.DesignFee?.ToString("#,##0") ?? "");
                    SetFieldWithFontSize("FranchiseFee", contract.FranchiseFee?.ToString("#,##0") ?? "");
                  //  SetFieldWithFontSize("TotalMoneyParse", totalMoneyParse);
                 //   SetFieldWithFontSize("DepositParse", depositParse);
                 //   SetFieldWithFontSize("DesignFeeParse", designFeeParse);
                  //  SetFieldWithFontSize("FranchiseFeeParse", FranchiseFeeParse);
                    SetFieldWithFontSize("ContractCode", contract.ContractCode ?? "");

                    stamper.FormFlattening = true;
                }
            }

            var finalOutputStream = new MemoryStream(outputStream.ToArray());
            finalOutputStream.Position = 0;
            return finalOutputStream;
        }

        private async Task<Stream> DownloadFileFromFirebaseAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Kiểm tra xem phản hồi có thành công không.

            var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Đặt lại vị trí của stream về đầu.
            return memoryStream;
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
                return result + " đồng";
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