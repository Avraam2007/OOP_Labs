using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class Tools {
        public static bool ValidateArray<T>(IEnumerable<T> arr) {
            return (arr is null || !arr.Any());
        }

        public static string GetTypeName<T>(T obj) {
            return obj.GetType().Name;
        }
    }
}
