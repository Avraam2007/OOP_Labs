using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Utilities;

namespace OOP_UnitTests {
    [TestClass]
    public class UserServiceTest {
        [TestMethod]
        [TestCategory("HashPassword")]
        public void HashPassword_ValidString_ReturnsExpectedSha256Hex() {
            string expectedHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f";

            string actualHash = UserService.HashPassword("password123");

            Assert.AreEqual(expectedHash, actualHash);
        }

        [TestMethod]
        [TestCategory("HashPassword")]
        public void HashPassword_DifferentCasing_ReturnsDifferentHashes() {
            string hashLower = UserService.HashPassword("secret");
            string hashUpper = UserService.HashPassword("Secret");

            Assert.AreNotEqual(hashLower, hashUpper);
        }

        [TestMethod]
        [TestCategory("CheckPassword")]
        public void CheckPassword_CorrectPassword_ReturnsTrue() {
            string rawPassword = "MySecurePassword!";
            string hashedPassword = UserService.HashPassword(rawPassword);
            var user = new User(1, "testuser", hashedPassword);

            bool isValid = UserService.CheckPassword(rawPassword, user);

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        [TestCategory("CheckPassword")]
        public void CheckPassword_IncorrectPassword_ReturnsFalse() {
            string hashedPassword = UserService.HashPassword("CorrectPassword");
            var user = new User(1, "testuser", hashedPassword);

            bool isValid = UserService.CheckPassword("WrongPassword", user);

            Assert.IsFalse(isValid);
        }
    }
}
