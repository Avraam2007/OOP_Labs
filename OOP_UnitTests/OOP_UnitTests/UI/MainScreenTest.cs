using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.UI;
using System.Reflection;

namespace OOP_UnitTests.UI {
    [TestClass]
    public class MainScreenTests {
        #region Private Layout Helper Methods Tests

        [TestMethod]
        [DataRow(-1, 1, 1, 0)]
        [DataRow(1, 1, -1, 0)]
        [DataRow(0, 1, 0, 0)]
        [DataRow(1, 1, 1, 10)] // 1 * (5 + 2) + 1 * 3 = 10
        [DataRow(2, 1, 1, 17)] // 2 * (5 + 2) + 1 * 3 = 17
        [DataRow(3, 1, 1, 24)] // 3 * (5 + 2) + 1 * 3 = 24
        public void GetHeaderHeight_VariousParameters_ReturnsExpectedHeight(
            int panelRows, int verticalPadding, int rules, int expectedHeight) {
            var method = typeof(MainScreen).GetMethod("GetHeaderHeight", BindingFlags.NonPublic | BindingFlags.Static);

            int actualHeight = (int)method.Invoke(null, new object[] { panelRows, verticalPadding, rules });

            Assert.AreEqual(expectedHeight, actualHeight);
        }

        [TestMethod]
        [DataRow(0, 2, 5, 0)]  // selectedIndex < topIndex -> повертає selectedIndex
        [DataRow(2, 2, 5, 2)]  // selectedIndex у межах сторінки -> залишається topIndex
        [DataRow(6, 2, 5, 2)]  // останній видимий елемент (2 + 5 - 1 = 6) -> залишається topIndex
        [DataRow(7, 2, 5, 3)]  // selectedIndex >= topIndex + pageSize -> зсув на 7 - 5 + 1 = 3
        public void GetTopIndex_ScrollingWindow_CalculatesExpectedTopIndex(
            int selectedIndex, int topIndex, int pageSize, int expectedTopIndex) {
            var method = typeof(MainScreen).GetMethod("GetTopIndex", BindingFlags.NonPublic | BindingFlags.Static);

            int actualTopIndex = (int)method.Invoke(null, new object[] { selectedIndex, topIndex, pageSize });

            Assert.AreEqual(expectedTopIndex, actualTopIndex);
        }

        [TestMethod]
        [DataRow(null, 0)]
        [DataRow("", 0)]
        [DataRow("Plain text", 10)]
        [DataRow("Welcome to [green]ECommerce[/]", 20)]
        [DataRow("[bold red]Error[/] [yellow]Warning[/]", 13)]
        public void GetRawTextLength_MarkupStrings_StripsTagsAndReturnsVisibleLength(
            string markupText, int expectedLength) {
            var method = typeof(MainScreen).GetMethod("GetRawTextLength", BindingFlags.NonPublic | BindingFlags.Static);

            int actualLength = (int)method.Invoke(null, new object[] { markupText });

            Assert.AreEqual(expectedLength, actualLength);
        }

        [TestMethod]
        [DataRow(-1, 2, 1, 0)]
        [DataRow(10, -1, 1, 0)]
        [DataRow(10, 2, -1, 0)]
        [DataRow(0, 0, 0, 0)]
        [DataRow(10, 2, 1, 16)] // 10 + 2 * (2 + 1) = 16
        public void GetPanelWidth_VariousDimensions_ReturnsExpectedWidth(
            int contentLength, int horizontalPadding, int frameWidth, int expectedWidth) {
            var method = typeof(MainScreen).GetMethod("GetPanelWidth", BindingFlags.NonPublic | BindingFlags.Static);

            int actualWidth = (int)method.Invoke(null, new object[] { contentLength, horizontalPadding, frameWidth });

            Assert.AreEqual(expectedWidth, actualWidth);
        }

        #endregion

        #region ShowMenuWithLiveClock Guard Tests

        [TestMethod]
        public void ShowMenuWithLiveClock_NullOptionsArray_ReturnsDefaultImmediately() {
            var user = new User(1, "TestUser", "pass1234");

            string result = MainScreen.ShowMenuWithLiveClock<string>("Header", null, user);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShowMenuWithLiveClock_EmptyOptionsArray_ReturnsDefaultImmediately() {
            var user = new User(1, "TestUser", "pass1234");

            int result = MainScreen.ShowMenuWithLiveClock("Header", new int[0], user);

            Assert.AreEqual(0, result);
        }

        #endregion
    }
}
