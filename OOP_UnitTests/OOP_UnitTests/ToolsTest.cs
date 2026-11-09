using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_UnitTests {
    [TestClass]
    public class ToolsTest {
        [TestMethod]
        public void Tools_WithValidEmail() {
            string testEmail = "goodle@glow.mers.com";

            var actual = Tools.IsValidEmail(testEmail);

            Assert.IsTrue(actual);
        }


        [TestMethod]
        public void Tools_WithInvalidEmail() {
            string testEmail = "fsdggdfsg";

            var actual = Tools.IsValidEmail(testEmail);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void Tools_WithCodePhoneNumber() {
            string testPhone = "+380695889898";

            var actual = Tools.IsValidPhone(testPhone);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Tools_WithoutCodePhoneNumber() {
            string testPhone = "0695889898";

            var actual = Tools.IsValidPhone(testPhone);

            Assert.IsTrue(actual);
        }


        [TestMethod]
        public void Tools_WithInvalidPhoneNumber() {
            string testPhone = "46516446";

            var actual = Tools.IsValidEmail(testPhone);

            Assert.IsFalse(actual);
        }
    }
}
