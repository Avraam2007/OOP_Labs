using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OOP_Main {
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
    }
}
