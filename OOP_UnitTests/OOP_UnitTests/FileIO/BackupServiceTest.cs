using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.FileIO;
using System;
using System.IO;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class BackupServiceTests {
        private string _tempRootDir;
        private string _sourceDir;
        private string _backupDir;
        private string _restoreDir;

        [TestInitialize]
        public void Setup() {
            _tempRootDir = Path.Combine(Path.GetTempPath(), "OOP_BackupTests_" + Guid.NewGuid().ToString("N"));
            _sourceDir = Path.Combine(_tempRootDir, "SourceData");
            _backupDir = Path.Combine(_tempRootDir, "Backups");
            _restoreDir = Path.Combine(_tempRootDir, "RestoredData");

            Directory.CreateDirectory(_sourceDir);
        }

        [TestCleanup]
        public void Cleanup() {
            if (Directory.Exists(_tempRootDir)) {
                Directory.Delete(_tempRootDir, recursive: true);
            }
        }

        #region CreateZipBackup Tests

        [TestMethod]
        public void CreateZipBackup_ValidDirectories_CreatesZipArchiveAndReturnsPath() {
            string sampleFile = Path.Combine(_sourceDir, "data.txt");
            File.WriteAllText(sampleFile, "Important store data");

            string createdZipPath = BackupService.CreateZipBackup(_sourceDir, _backupDir);

            Assert.IsFalse(string.IsNullOrWhiteSpace(createdZipPath));
            Assert.IsTrue(File.Exists(createdZipPath));
            Assert.StartsWith("backup_", Path.GetFileName(createdZipPath));
            Assert.AreEqual(".zip", Path.GetExtension(createdZipPath));
        }

        [TestMethod]
        public void CreateZipBackup_MoreThanFiveBackupsExist_KeepsOnlyFiveLatestZipFiles() {
            Directory.CreateDirectory(_backupDir);

            // Створюємо 5 старих фіктивних архівів з давнішим часом створення
            for (int i = 1; i <= 5; i++) {
                string oldZip = Path.Combine(_backupDir, $"old_backup_{i}.zip");
                File.WriteAllText(oldZip, "dummy");
                File.SetCreationTime(oldZip, DateTime.Now.AddMinutes(-10 - i));
            }

            File.WriteAllText(Path.Combine(_sourceDir, "item.txt"), "content");

            string newestZip = BackupService.CreateZipBackup(_sourceDir, _backupDir);

            string[] remainingZips = Directory.GetFiles(_backupDir, "*.zip");
            Assert.HasCount(5, remainingZips);
            Assert.IsTrue(File.Exists(newestZip));
            Assert.IsFalse(File.Exists(Path.Combine(_backupDir, "old_backup_5.zip")));
        }

        [TestMethod]
        public void CreateZipBackup_TargetFolderInsideSourceDirectory_ThrowsECommerceException() {
            // Спроба створити архів всередині тієї самої директорії, яка архівується, викликає IOException -> ECommerceException
            string nestedBackupDir = Path.Combine(_sourceDir, "SubBackups");

            Assert.Throws<ECommerceException>(() =>
                BackupService.CreateZipBackup(_sourceDir, nestedBackupDir));
        }

        #endregion

        #region RestoreFromZipBackup Tests

        [TestMethod]
        public void RestoreFromZipBackup_ExistingZipAndDestination_OverwritesAndExtractsAllFiles() {
            string originalFile = Path.Combine(_sourceDir, "users.json");
            File.WriteAllText(originalFile, "[{\"Id\":1}]");
            string zipPath = BackupService.CreateZipBackup(_sourceDir, _backupDir);

            // Попередньо створюємо застарілий файл у директорії відновлення, щоб перевірити її очищення
            Directory.CreateDirectory(_restoreDir);
            string staleFile = Path.Combine(_restoreDir, "stale.txt");
            File.WriteAllText(staleFile, "old data");

            BackupService.RestoreFromZipBackup(zipPath, _restoreDir);

            string restoredFile = Path.Combine(_restoreDir, "users.json");
            Assert.IsTrue(File.Exists(restoredFile));
            Assert.AreEqual("[{\"Id\":1}]", File.ReadAllText(restoredFile));
            Assert.IsFalse(File.Exists(staleFile));
        }

        [TestMethod]
        public void RestoreFromZipBackup_NonExistentZipFile_ThrowsFileNotFoundException() {
            string missingZip = Path.Combine(_backupDir, "does_not_exist.zip");

            Assert.Throws<FileNotFoundException>(() =>
                BackupService.RestoreFromZipBackup(missingZip, _restoreDir));
        }

        #endregion

        #region GetLatestBackupFilePath Tests

        [TestMethod]
        public void GetLatestBackupFilePath_NonExistentFolder_ReturnsNull() {
            string missingFolder = Path.Combine(_tempRootDir, "NoSuchFolder");

            string result = BackupService.GetLatestBackupFilePath(missingFolder);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetLatestBackupFilePath_EmptyFolderOrOnlyNonZipFiles_ReturnsNull() {
            Directory.CreateDirectory(_backupDir);
            File.WriteAllText(Path.Combine(_backupDir, "notes.txt"), "not a zip");

            string result = BackupService.GetLatestBackupFilePath(_backupDir);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetLatestBackupFilePath_MultipleZipFiles_ReturnsNewestByCreationTime() {
            Directory.CreateDirectory(_backupDir);

            string olderZip = Path.Combine(_backupDir, "backup_old.zip");
            string newerZip = Path.Combine(_backupDir, "backup_new.zip");

            File.WriteAllText(olderZip, "old");
            File.WriteAllText(newerZip, "new");

            File.SetCreationTime(olderZip, DateTime.Now.AddHours(-2));
            File.SetCreationTime(newerZip, DateTime.Now);

            string latestPath = BackupService.GetLatestBackupFilePath(_backupDir);

            Assert.AreEqual(newerZip, latestPath);
        }

        #endregion
    }
}
