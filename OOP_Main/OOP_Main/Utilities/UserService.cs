using OOP_Main.Entities;
using System.Security.Cryptography;
using System.Text;

namespace OOP_Main.Utilities {
    public readonly struct UserService {
        public static string HashPassword(in string password) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) {
                    builder.Append(b.ToString("x2")); // Convert to hexadecimal string
                }
                return builder.ToString();
            }
        }

        public static bool CheckPassword(in string passwordToCheck, in User user) {
            return HashPassword(passwordToCheck) == user.Password;
        }
    }
}
