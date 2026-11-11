using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OOP_UnitTests {
    [TestClass]
    public class ToolsTest {

        #region ValidateEmail Tests

        [TestMethod]
        [TestCategory("ValidateEmail")]
        public void IsValidEmail_WithValidEmail_ReturnsTrue() {
            string testEmail = "goodle@glow.mers.com";

            var actual = Tools.IsValidEmail(testEmail);

            Assert.IsTrue(actual);
        }


        [TestMethod]
        [TestCategory("ValidateEmail")]
        public void IsValidEmail_WithInvalidEmail_ReturnsFalse() {
            string testEmail = "fsdggdfsg";

            var actual = Tools.IsValidEmail(testEmail);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [TestCategory("ValidateEmail")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void IsValidEmail_WithNullOrEmptyEmail_ReturnsFalse(string testEmail) {
            var actual = Tools.IsValidEmail(testEmail);

            Assert.IsFalse(actual);
        }

        #endregion

        #region ValidatePhone Tests

        [TestMethod]
        [TestCategory("ValidatePhone")]
        [DataRow("+380695889898")]
        [DataRow("380695889898")]
        public void IsValidPhone_WithRegionCodePhoneNumber_ReturnsTrue(string testPhone) {
            var actual = Tools.IsValidPhone(testPhone);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [TestCategory("ValidatePhone")]
        public void IsValidPhone_WithoutRegionCodePhoneNumber_ReturnsTrue() {
            string testPhone = "0695889898";

            var actual = Tools.IsValidPhone(testPhone);

            Assert.IsTrue(actual);
        }


        [TestMethod]
        [TestCategory("ValidatePhone")]
        public void IsValidPhone_WithInvalidPhoneNumber_ReturnsFalse() {
            string testPhone = "46516446";

            var actual = Tools.IsValidPhone(testPhone);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [TestCategory("ValidatePhone")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void IsValidPhone_WithNullOrEmptyPhoneNumber_ReturnsFalse(string testPhone) {
            var actual = Tools.IsValidPhone(testPhone);

            Assert.IsFalse(actual);
        }

        #endregion

        #region ValidateArticle Tests

        [TestMethod]
        [TestCategory("ValidateArticle")]
        [DataRow("FN-0001")]
        [DataRow("SF-0001")]
        [DataRow("EL-0001")]
        [DataRow("CL-0001")]
        [DataRow("PR-0001")]
        [DataRow("BDL-0001")]
        public void IsValidArticle_WithValidBaseArticle_ReturnsTrue(string testArticle) {
            var actual = Tools.IsValidArticle(testArticle);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [TestCategory("ValidateArticle")]
        [DataRow("DISC10-EL-0001")]
        [DataRow("DISC15.5-SF-1234")]
        [DataRow("EXTWAR2-FN-0001")]
        [DataRow("DEC5-CL-9999")]
        [DataRow("EXTWAR12-DISC10.5-BDL-0001")]
        public void IsValidArticle_WithValidDecoratedArticles_ReturnsTrue(string testArticle) {
            var actual = Tools.IsValidArticle(testArticle);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [TestCategory("ValidateArticle")]
        [DataRow("RV-008")]
        [DataRow("EL-001")]
        [DataRow("EL-00001")]
        [DataRow("DISC-EL-0001")]
        public void IsValidArticle_WithInvalidArticle_ReturnsFalse(string testArticle) {
            var actual = Tools.IsValidArticle(testArticle);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [TestCategory("ValidateArticle")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void IsValidArticle_WithNullOrEmptyArticle_ReturnsFalse(string testArticle) {
            var actual = Tools.IsValidPhone(testArticle);

            Assert.IsFalse(actual);
        }

        #endregion

        #region GetTypeName Tests

        [TestMethod]
        [TestCategory("GetTypeName")]
        public void Tools_GetTypeName() {
            var testObj = new Order();
            string expected = "Order";

            var actual = Tools.GetTypeName(testObj);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("GetTypeName")]
        [ExpectedException(typeof(NullReferenceException))]
        public void Tools_GetTypeName_WithNullThrowsException() {
            Order testObj = null;

            Tools.GetTypeName(testObj);
        }

        #endregion

        #region GetCurrencyOutput Tests

        [TestMethod]
        [TestCategory("GetCurrencyOutput")]
        public void Tools_GetCurrencyOutput() {
            double testValue = 1.9942652;
            string expected = testValue.ToString("C2", CultureInfo.CreateSpecificCulture("en-US"));

            var actual = Tools.GetCurrencyOutput(testValue);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region RoundNumber Tests

        [TestMethod]
        [TestCategory("RoundNumber")]
        public void Tools_RoundDown() {
            double testValue = 1.9942652;
            double expected = 1.99;

            var actual = Tools.RoundNumber(testValue);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("RoundNumber")]
        public void Tools_RoundUp() {
            double testValue = 1.9962652;
            double expected = 2;

            var actual = Tools.RoundNumber(testValue);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("RoundNumber")]
        public void Tools_RoundMidpointAwayFromZero() {
            double testValue = 2.345;
            double expected = 2.35;

            var actual = Tools.RoundNumber(testValue);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("RoundNumber")]
        public void Tools_RoundNumber_CustomDigits() {
            double testValue = 1.9942652;
            double expected = 1.994;

            var actual = Tools.RoundNumber(testValue, 3);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("RoundPrice")]
        public void Tools_RoundPrice_DefaultAndCustomDigits() {
            double testPrice = 15.6789;

            Assert.AreEqual(15.68, Tools.RoundPrice(testPrice));
            Assert.AreEqual(15.7, Tools.RoundPrice(testPrice, 1));
        }

        #endregion

        #region AddSpacesBetweenCapitalizedWords Tests

        [TestMethod]
        [TestCategory("AddSpacesBetweenCapitalizedWords")]
        public void Tools_CheckSpaces() {
            string testValue = "ThisIsTestString";
            string expected = "This Is Test String";

            var actual = Tools.AddSpacesBetweenCapitalizedWords(testValue);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("AddSpacesBetweenCapitalizedWords")]
        [DataRow(null)]
        [DataRow("")]
        public void Tools_CheckSpaces_WithNullOrEmpty(string testValue) {
            var actual = Tools.AddSpacesBetweenCapitalizedWords(testValue);

            Assert.AreEqual(testValue, actual);
        }

        #endregion

        #region ValidateArray Tests

        [TestMethod]
        [TestCategory("ValidateArray")]
        public void Tools_ValidArray() {
            var testArray = new List<dynamic> { 1, "", null};

            var actual = Tools.ValidateArray(testArray);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [TestCategory("ValidateArray")]
        public void Tools_NullArray() {
            List<dynamic> testArray = null;

            var actual = Tools.ValidateArray(testArray);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [TestCategory("ValidateArray")]
        public void Tools_EmptyArray() {
            var testArray = new List<dynamic>();

            var actual = Tools.ValidateArray(testArray);

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
