using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.FileIO;
using System;
using System.IO;
using System.Text;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class FileServiceTests {
        private string _tempDir;

        [TestInitialize]
        public void Setup() {
            _tempDir = Path.Combine(Path.GetTempPath(), "OOP_FileServiceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void Cleanup() {
            if (Directory.Exists(_tempDir)) {
                Directory.Delete(_tempDir, recursive: true);
            }
        }

        #region EnsureDirectoryExists Tests

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void EnsureDirectoryExists_NullOrEmptyPath_DoesNotThrow(string invalidPath) {
            FileService.EnsureDirectoryExists(invalidPath);
        }

        [TestMethod]
        public void EnsureDirectoryExists_NewNestedDirectory_CreatesDirectory() {
            string nestedDir = Path.Combine(_tempDir, "Level1", "Level2");

            FileService.EnsureDirectoryExists(nestedDir);

            Assert.IsTrue(Directory.Exists(nestedDir));
        }

        [TestMethod]
        public void EnsureDirectoryExists_AlreadyExistingDirectory_DoesNotThrowAndPreservesFiles() {
            string existingFile = Path.Combine(_tempDir, "keep.txt");
            File.WriteAllText(existingFile, "data");

            FileService.EnsureDirectoryExists(_tempDir);

            Assert.IsTrue(Directory.Exists(_tempDir));
            Assert.IsTrue(File.Exists(existingFile));
        }

        #endregion

        #region File Size & Line Count Metrics Tests

        [TestMethod]
        public void GetFileSizeInBytes_NonExistentFile_ReturnsZero() {
            string missingFile = Path.Combine(_tempDir, "missing.txt");

            long size = FileService.GetFileSizeInBytes(missingFile);

            Assert.AreEqual(0, size);
        }

        [TestMethod]
        public void GetFileSizeInBytes_ExistingFile_ReturnsExactByteLength() {
            string filePath = Path.Combine(_tempDir, "bytes.txt");
            byte[] content = Encoding.UTF8.GetBytes("Hello World!"); // 12 bytes
            File.WriteAllBytes(filePath, content);

            long size = FileService.GetFileSizeInBytes(filePath);

            Assert.AreEqual(12, size);
        }

        [TestMethod]
        public void GetFileSizeInLines_NonExistentFile_ReturnsZero() {
            string missingFile = Path.Combine(_tempDir, "missing_lines.txt");

            int lines = FileService.GetFileSizeInLines(missingFile);

            Assert.AreEqual(0, lines);
        }

        [TestMethod]
        public void GetFileSizeInLines_EmptyFile_ReturnsZero() {
            string emptyFile = Path.Combine(_tempDir, "empty.txt");
            File.WriteAllText(emptyFile, string.Empty);

            int lines = FileService.GetFileSizeInLines(emptyFile);

            Assert.AreEqual(0, lines);
        }

        [TestMethod]
        public void GetFileSizeInLines_MultilineFile_ReturnsExactLineCount() {
            string multilineFile = Path.Combine(_tempDir, "lines.txt");
            File.WriteAllLines(multilineFile, new[] { "First line", "Second line", "Third line", "Fourth line" });

            int lines = FileService.GetFileSizeInLines(multilineFile);

            Assert.AreEqual(4, lines);
        }

        #endregion

        #region DeleteOldFiles Tests

        [TestMethod]
        public void DeleteOldFiles_NonExistentFolder_ReturnsZero() {
            string missingFolder = Path.Combine(_tempDir, "NoSuchDir");

            int deleted = FileService.DeleteOldFiles(missingFolder, "*.log", 3);

            Assert.AreEqual(0, deleted);
        }

        [TestMethod]
        public void DeleteOldFiles_FileCountWithinLimit_DeletesNothing() {
            File.WriteAllText(Path.Combine(_tempDir, "file1.log"), "1");
            File.WriteAllText(Path.Combine(_tempDir, "file2.log"), "2");

            int deleted = FileService.DeleteOldFiles(_tempDir, "*.log", maxFilesToKeep: 3);

            Assert.AreEqual(0, deleted);
            Assert.HasCount(2, Directory.GetFiles(_tempDir, "*.log"));
        }

        [TestMethod]
        public void DeleteOldFiles_ExceedingLimit_DeletesOldestMatchingFilesAndKeepsNewest() {
            string oldestFile = Path.Combine(_tempDir, "old_1.log");
            string olderFile = Path.Combine(_tempDir, "old_2.log");
            string newerFile = Path.Combine(_tempDir, "new_1.log");
            string newestFile = Path.Combine(_tempDir, "new_2.log");
            string unrelatedFile = Path.Combine(_tempDir, "other.txt");

            File.WriteAllText(oldestFile, "1");
            File.WriteAllText(olderFile, "2");
            File.WriteAllText(newerFile, "3");
            File.WriteAllText(newestFile, "4");
            File.WriteAllText(unrelatedFile, "keep");

            DateTime baseTime = DateTime.Now;
            File.SetCreationTime(oldestFile, baseTime.AddMinutes(-40));
            File.SetCreationTime(olderFile, baseTime.AddMinutes(-30));
            File.SetCreationTime(newerFile, baseTime.AddMinutes(-10));
            File.SetCreationTime(newestFile, baseTime);
            File.SetCreationTime(unrelatedFile, baseTime.AddHours(-5));

            int deleted = FileService.DeleteOldFiles(_tempDir, "*.log", maxFilesToKeep: 2);

            Assert.AreEqual(2, deleted);
            Assert.IsFalse(File.Exists(oldestFile));
            Assert.IsFalse(File.Exists(olderFile));
            Assert.IsTrue(File.Exists(newerFile));
            Assert.IsTrue(File.Exists(newestFile));
            Assert.IsTrue(File.Exists(unrelatedFile));
        }

        #endregion
    }
}

