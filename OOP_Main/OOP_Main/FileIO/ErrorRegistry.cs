using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.FileIO {
    public delegate void OnError(string message);
    public static class ErrorRegistry {
        private static event OnError Taken;

        public static void RegisterHandler(OnError del) {
            Taken = del;
        }

        public static void HandleError(string message) {
            Taken?.Invoke(message);
        }
    }
}
