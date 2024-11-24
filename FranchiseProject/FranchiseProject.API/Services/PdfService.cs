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
            string firebaseUrl = "https://firebasestorage.googleapis.com/v0/b/franchise-project-1ea45.firebasestorage.app/o/Contract%2Fhop-dong-nhuong-quyen-thuong-mai_1010092534%20(3).pdf?alt=media&token=38cd36c7-a187-4716-891b-52a30132dc16";
            Stream pdfTemplateStream = await DownloadFileFromFirebaseAsync(firebaseUrl);
            var Deposit = contract.TotalMoney * 20;
            var totalMoneyParse = NumberToWordsConverter.ConvertToWords(contract.TotalMoney.Value);
            var depositParse = NumberToWordsConverter.ConvertToWords(Deposit.Value);
            var designFeeParse = NumberToWordsConverter.ConvertToWords(contract.DesignFee.Value);
            var FranchiseFeeParse = NumberToWordsConverter.ConvertToWords(contract.FranchiseFee.Value);

            var outputStream = new MemoryStream();
            using (var reader = new PdfReader(pdfTemplateStream))
            {
                using (var stamper = new PdfStamper(reader, outputStream))
                {
                   var form = stamper.AcroFields;
                    form.SetField("TotalMoney", contract.TotalMoney.ToString());
                     form.SetField("Deposit", Deposit.ToString());
                                   
                    form.SetField("DesignFee", contract.DesignFee.ToString());
                    form.SetField("FranchiseFee", contract.FranchiseFee.ToString());
                    form.SetField("TotalMoneyParse", totalMoneyParse.ToString());
                    form.SetField("DepositParse", depositParse.ToString());
                    form.SetField("DesignFeeParse", depositParse.ToString());
                    form.SetField("FrachiseFeeParse", FranchiseFeeParse.ToString());
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
            private static readonly string[] Units = { "", "mươi", "trăm", "ngàn", "triệu", "tỷ" };
            private static readonly string[] Numbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

            public static string ConvertToWords(double number)
            {
                if (number == 0) return "không";

                var words = new List<string>();
                var numStr = ((long)number).ToString();
                var length = numStr.Length;
                var groupCount = 0;

                while (length > 0)
                {
                    int startIndex = Math.Max(0, length - 3);
                    var group = numStr.Substring(startIndex, length - startIndex);
                    var groupValue = int.Parse(group);

                    if (groupValue > 0 || groupCount == 0)
                    {
                        var groupWords = ConvertGroupToWords(groupValue);
                        if (groupCount > 0)
                        {
                            groupWords += " " + Units[3 + (groupCount - 1) * 2];
                        }

                        words.Insert(0, groupWords.Trim());
                    }

                    length -= 3;
                    groupCount++;
                }

                return string.Join(" ", words).Trim();
            }

            private static string ConvertGroupToWords(int number)
            {
                var words = new List<string>();
                var hundreds = number / 100;
                var remainder = number % 100;
                var tens = remainder / 10;
                var units = remainder % 10;

                if (hundreds > 0)
                {
                    words.Add(Numbers[hundreds] + " trăm");
                }

                if (tens > 0)
                {
                    if (tens == 1)
                    {
                        words.Add("mười");
                    }
                    else
                    {
                        words.Add(Numbers[tens] + " mươi");
                    }
                }
                else if (remainder > 0 && hundreds > 0)
                {
                    words.Add("lẻ");
                }

                if (units > 0)
                {
                    if (tens > 1 && units == 5)
                    {
                        words.Add("lăm");
                    }
                    else
                    {
                        words.Add(Numbers[units]);
                    }
                }

                return string.Join(" ", words).Trim();
            }
        }
    }
}