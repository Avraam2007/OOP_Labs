using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Utilities;
using System;
using System.IO;
using System.Reflection;

namespace OOP_UnitTests.Utilities {
    [TestClass]
    public class LoggerTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_LoggerTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            ResetLoggerState();
        }

        [TestCleanup]
        public void Cleanup() {
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                Directory.Delete(_tempWorkingDir, recursive: true);
            }
        }

        private static void ResetLoggerState() {
            Type loggerType = typeof(Logger);

            FieldInfo lineCountField = loggerType.GetField("_currentLineCount", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo fileIndexField = loggerType.GetField("_currentFileIndex", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo filePathField = loggerType.GetField("_currentFilePath", BindingFlags.NonPublic | BindingFlags.Static);

            lineCountField.SetValue(null, 0);
            fileIndexField.SetValue(null, 1);

            string expectedInitialPath = Path.Combine("Logs", $"app_log_{DateTime.Now:ddMMyyyy}_1.log");
            filePathField.SetValue(null, expectedInitialPath);
        }

        #region Formatting & Log Levels Tests

        [TestMethod]
        public void Log_DefaultParameters_WritesInfoSystemEntryToLogFile() {
            Logger.Log("Application started");

            string expectedLogFile = Path.Combine("Logs", $"app_log_{DateTime.Now:ddMMyyyy}_1.log");
            Assert.IsTrue(File.Exists(expectedLogFile));

            string content = File.ReadAllText(expectedLogFile);
            Assert.Contains("|Info| : (System) Application started", content);
        }

        [TestMethod]
        [DataRow(Logger.LogLevels.Error, "Error")]
        [DataRow(Logger.LogLevels.Warn, "Warn")]
        [DataRow(Logger.LogLevels.Info, "Info")]
        [DataRow(Logger.LogLevels.Debug, "Debug")]
        [DataRow(Logger.LogLevels.Silly, "Silly")]
        [DataRow((Logger.LogLevels)999, "Unknown")]
        public void Log_AllLogLevelsAndCustomUsername_FormatsLevelAndUserCorrectly(
            Logger.LogLevels level, string expectedLevelText) {
            Logger.Log("Sample event", level, "AdminAlice");

            string logFile = Path.Combine("Logs", $"app_log_{DateTime.Now:ddMMyyyy}_1.log");
            string content = File.ReadAllText(logFile);

            Assert.Contains($"|{expectedLevelText}| : (AdminAlice) Sample event", content);
        }

        #endregion

        #region Log Rotation Tests

        [TestMethod]
        public void Log_ExceedingMaxLinesPerFile_CreatesNewIncrementedLogFile() {
            // Записуємо 100 рядків у перший файл (індекс 1)
            for (int i = 1; i <= 100; i++) {
                Logger.Log($"Entry #{i}", Logger.LogLevels.Debug);
            }

            string firstLogFile = Path.Combine("Logs", $"app_log_{DateTime.Now:ddMMyyyy}_1.log");
            string secondLogFile = Path.Combine("Logs", $"app_log_{DateTime.Now:ddMMyyyy}_2.log");

            Assert.IsTrue(File.Exists(firstLogFile));
            Assert.IsFalse(File.Exists(secondLogFile));

            // 101-й запис має створити другий файл логів (індекс 2)
            Logger.Log("Overflow entry #101", Logger.LogLevels.Warn, "System");

            Assert.IsTrue(File.Exists(secondLogFile));

            string[] firstFileLines = File.ReadAllLines(firstLogFile);
            string[] secondFileLines = File.ReadAllLines(secondLogFile);

            Assert.HasCount(100, firstFileLines);
            Assert.HasCount(1, secondFileLines);
            Assert.Contains("|Warn| : (System) Overflow entry #101", secondFileLines[0]);
        }

        #endregion
    }
}
