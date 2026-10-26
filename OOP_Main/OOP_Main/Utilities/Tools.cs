using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace OOP_Main.Utilities {
    public static class Tools {
        public static bool ValidateArray<T>(IEnumerable<T> arr) {
            return !(arr is null || !arr.Any());
        }

        public static string GetTypeName<T>(T obj) {
            return obj.GetType().Name;
        }

        public static string GetCurrencyOutput(this double value) {
            return value.ToString("C2", CultureInfo.CreateSpecificCulture("en-US"));
        }

        public static double RoundNumber(this double number, int digitsAfterPeriod = 2) {
            return Math.Round(number, digitsAfterPeriod, MidpointRounding.AwayFromZero);
        }

        public static double RoundPrice(this double price, int digitsAfterPeriod = 2) {
            return RoundNumber(price, digitsAfterPeriod);
        }


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
