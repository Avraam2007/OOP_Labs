using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OOP_Main.Utilities {
    public static class Tools {
        public static bool ValidateArray<T>(IEnumerable<T> arr) {
            return (arr is null || !arr.Any());
        }

        public static string GetTypeName<T>(T obj) {
            return obj.GetType().Name;
        }

        public static Func<double, double> RoundPrice = price => {
            return Math.Round(price, 2, MidpointRounding.AwayFromZero);
        };

        public static string AddSpacesBetweenCapitalizedWords(this string text) {
            if (string.IsNullOrEmpty(text)) return text;

            string ans = Regex.Replace(text, "([A-Z])", " $1", RegexOptions.Compiled);
            return ans;
        }

        public static bool IsValidArticle(string article) => ValidationService.IsValidArticle(article);

        public static bool IsValidEmail(string email) => ValidationService.IsValidEmail(email);

        public static bool IsValidPhone(string phone) => ValidationService.IsValidPhone(phone);
    }
}
