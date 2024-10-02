using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.RefreshTokenViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Utils
{
    public static class GenerateUserCredentialsString
    {
        
        public static UserLoginModel GenerateUserCredentials(this User user)
        {
            var normalizedString = user.FullName.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            var fullname = stringBuilder.ToString();    
            var words = fullname.Split(' ');
            string lastName = words.Last();
            
            
            var initials = string.Concat(words.Take(words.Length - 1).Select(n => n[0]));
            var year  = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month.ToString();
            var username = lastName + initials + year + month;
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var password = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            
            return new UserLoginModel { Password = password, UserName =username };
        }
    }
}
