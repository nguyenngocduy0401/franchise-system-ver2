using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Utils
{
    public static class StringExtensions
    {
        public static string GetTextWithoutHtml(this string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent)) return "";
            string plainText = Regex.Replace(htmlContent, "<.*?>", String.Empty);
            return plainText;
        }
    }
}
