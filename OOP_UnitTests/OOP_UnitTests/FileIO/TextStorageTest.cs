using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.FileIO;
using OOP_Main.Interfaces;
using System;
using System.IO;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class TextStorageAndAdapterTests {
        private string _tempDir;
        private TextStorage _textStorage;

        [TestInitialize]
        public void Setup() {
            _tempDir = Path.Combine(Path.GetTempPath(), "OOP_TextStorageTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
            _textStorage = new TextStorage();
        }

        [TestCleanup]
        public void Cleanup() {
            if (Directory.Exists(_tempDir)) {
                Directory.Delete(_tempDir, recursive: true);
            }
        }

        #region TextStorage SaveToFile & LoadFromFile Tests

        [TestMethod]
        public void LoadFromFile_NonExistentFile_ReturnsEmptyString() {
            string missingPath = Path.Combine(_tempDir, "missing.txt");

            string result = _textStorage.LoadFromFile(missingPath);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void SaveToFileAndLoadFromFile_NewNestedPath_CreatesDirectoryAndReadsContent() {
            string nestedPath = Path.Combine(_tempDir, "Logs", "receipt.txt");

            _textStorage.SaveToFile("Order #101 Paid", nestedPath);

            Assert.IsTrue(File.Exists(nestedPath));
            string loaded = _textStorage.LoadFromFile(nestedPath);
            Assert.AreEqual("Order #101 Paid" + Environment.NewLine, loaded);
        }

        [TestMethod]
        public void SaveToFile_AppendLineFalse_OverwritesExistingContent() {
            string filePath = Path.Combine(_tempDir, "overwrite.txt");

            _textStorage.SaveToFile("First line", filePath, appendLine: false);
            _textStorage.SaveToFile("Second line", filePath, appendLine: false);

            string loaded = _textStorage.LoadFromFile(filePath);
            Assert.AreEqual("Second line" + Environment.NewLine, loaded);
        }

        [TestMethod]
        public void SaveToFile_AppendLineTrue_AppendsContentToExistingFile() {
            string filePath = Path.Combine(_tempDir, "append.txt");

            _textStorage.SaveToFile("Line 1", filePath, appendLine: true);
            _textStorage.SaveToFile("Line 2", filePath, appendLine: true);

            string loaded = _textStorage.LoadFromFile(filePath);
            string expected = "Line 1" + Environment.NewLine + "Line 2" + Environment.NewLine;
            Assert.AreEqual(expected, loaded);
        }

        #endregion

        #region TextStorage Exception Handling Tests

        [TestMethod]
        public void LoadFromFile_LockedByAnotherStream_HandlesIOExceptionAndReturnsEmptyString() {
            string lockedPath = Path.Combine(_tempDir, "locked_read.txt");

            using (var lockStream = new FileStream(lockedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                string result = _textStorage.LoadFromFile(lockedPath);

                Assert.AreEqual(string.Empty, result);
            }
        }

        [TestMethod]
        public void SaveToFile_LockedByAnotherStream_HandlesIOExceptionWithoutThrowing() {
            string lockedPath = Path.Combine(_tempDir, "locked_write.txt");

            using (var lockStream = new FileStream(lockedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                _textStorage.SaveToFile("Blocked text", lockedPath);
            }

            Assert.IsTrue(File.Exists(lockedPath));
        }

        [TestMethod]
        public void SaveToFile_ReadOnlyFile_HandlesUnauthorizedAccessExceptionWithoutThrowing() {
            string readOnlyPath = Path.Combine(_tempDir, "readonly.txt");
            File.WriteAllText(readOnlyPath, "Initial content");
            File.SetAttributes(readOnlyPath, FileAttributes.ReadOnly);

            try {
                _textStorage.SaveToFile("Modified content", readOnlyPath);

                Assert.AreEqual("Initial content", File.ReadAllText(readOnlyPath));
            }
            finally {
                File.SetAttributes(readOnlyPath, FileAttributes.Normal);
            }
        }

        #endregion

        #region TextStorageAdapter Tests

        [TestMethod]
        public void TextStorageAdapter_SaveAndLoadThroughIStorageInterface_AdaptsArgumentOrderProperly() {
            IStorage<string> adapter = new TextStorageAdapter(_textStorage);
            string filePath = Path.Combine(_tempDir, "adapted.txt");

            // Адаптер приймає (filePath, data) згідно з IStorage<string> та делегує у TextStorage.SaveToFile(data, filePath)
            adapter.SaveToFile(filePath, "Adapted content");
            string loaded = adapter.LoadFromFile(filePath);

            Assert.AreEqual("Adapted content" + Environment.NewLine, loaded);
        }

        #endregion
    }
}
