using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using iTextSharp.text;
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
        
        public async Task<Stream> FillDocumentTemplate(InputContractViewModel contract)
        {
            string templatePath = _configuration["ContractTemplateUrl"];

            using (var doc = DocX.Load(templatePath))
            {
                var Deposit =contract.Deposit;
                var totalMoneyParse = contract.TotalMoney.HasValue ? NumberToWordsConverter.ConvertToWords(contract.TotalMoney.Value) : "";
                var depositParse = Deposit.HasValue ? NumberToWordsConverter.ConvertToWords(Deposit.Value) : "";
                var designFeeParse = contract.DesignFee.HasValue ? NumberToWordsConverter.ConvertToWords(contract.DesignFee.Value) : "";
                var FranchiseFeeParse = contract.FranchiseFee.HasValue ? NumberToWordsConverter.ConvertToWords(contract.FranchiseFee.Value) : "";
                var EquipmentFeeParse = contract.EquipmentFee.HasValue ? NumberToWordsConverter.ConvertToWords(contract.FranchiseFee.Value) : "";
                doc.ReplaceText("{{TotalMoney}}", contract.TotalMoney?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{Deposit}}", Deposit?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{DesignFee}}", contract.DesignFee?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{FranchiseFee}}", contract.FranchiseFee?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{TotalMoneyParse}}", totalMoneyParse);
                doc.ReplaceText("{{DepositParse}}", depositParse);
                doc.ReplaceText("{{DesignFeeParse}}", designFeeParse);
                doc.ReplaceText("{{FranchiseFeeParse}}", FranchiseFeeParse);
                doc.ReplaceText("{{ContractCode}}", contract.ContractCode ?? "");
                doc.ReplaceText("{{EquipmentFee}}", contract.EquipmentFee?.ToString("#,##0") ?? "");
                doc.ReplaceText("{{EquipmentFeeParse}}", EquipmentFeeParse ?? "");
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
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); 

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