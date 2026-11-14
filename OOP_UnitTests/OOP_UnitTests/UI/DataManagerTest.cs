using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.FileIO;
using OOP_Main.Repositories;
using OOP_Main.UI;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace OOP_UnitTests.UI {
    [TestClass]
    public class DataManagerTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_DataManagerTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            AnsiConsole.Console = _testConsole;

            ErrorRegistry.RegisterHandler(null);
        }

        [TestCleanup]
        public void Cleanup() {
            ErrorRegistry.RegisterHandler(null);
            AnsiConsole.Console = _originalConsole;
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                Directory.Delete(_tempWorkingDir, recursive: true);
            }
        }

        #region UploadFilesFrom & Backup Restoration Tests

        [TestMethod]
        public async Task UploadFilesFrom_MissingFilesAndNoBackups_OutputsWarningAndLoadsEmptyData() {
            var data = new Data();

            await RunAsyncVoid(() => DataManager.UploadFilesFrom(data));

            Assert.Contains("Data files are missing, and no backups were found in the Backups folder. New \nfiles will be created.\n", _testConsole.Output);
        }

        [TestMethod]
        public async Task UploadFilesFrom_MissingFilesWithValidBackup_RestoresFilesAndLoadsData() {
            // 1. Готуємо тимчасову папку з валідними JSON-файлами та пакуємо її у Backups/backup_test.zip
            string stagingDir = Path.Combine(_tempWorkingDir, "Staging");
            Directory.CreateDirectory(stagingDir);

            var seedData = new Data();
            seedData.Users.Add(new User(1, "RestoredAdmin", "pass1234", isAdmin: true));
            await seedData.SaveAllData();

            // Копіюємо згенеровані файли з DataStorage до архіву в Backups і видаляємо DataStorage
            string backupDir = Path.Combine(_tempWorkingDir, "Backups");
            Directory.CreateDirectory(backupDir);
            string zipPath = Path.Combine(backupDir, "backup_valid.zip");
            ZipFile.CreateFromDirectory(Path.Combine(_tempWorkingDir, "DataStorage"), zipPath);

            Directory.Delete(Path.Combine(_tempWorkingDir, "DataStorage"), recursive: true);

            // 2. Викликаємо UploadFilesFrom на новому екземплярі Data
            var targetData = new Data();
            await RunAsyncVoid(() => DataManager.UploadFilesFrom(targetData));

            Assert.Contains("Missing data files detected! Restoring from archive...", _testConsole.Output);
            Assert.Contains("Data is restored successfully: backup_valid.zip", _testConsole.Output);
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "users.json")));
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "products.json")));
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "orders.json")));
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "suppliers.json")));
        }

        [TestMethod]
        public async Task UploadFilesFrom_CorruptedBackupArchive_CatchesExceptionAndOutputsError() {
            string backupDir = Path.Combine(_tempWorkingDir, "Backups");
            Directory.CreateDirectory(backupDir);
            string brokenZipPath = Path.Combine(backupDir, "backup_corrupted.zip");
            File.WriteAllText(brokenZipPath, "not_a_valid_zip_archive");

            var data = new Data();
            await RunAsyncVoid(() => DataManager.UploadFilesFrom(data));

            Assert.Contains("Missing data files detected! Restoring from archive...", _testConsole.Output);
            Assert.Contains("Failed to restore from the archive:", _testConsole.Output);
        }

        [TestMethod]
        public async Task UploadFilesFrom_AllRequiredFilesExist_DoesNotTriggerBackupRestore() {
            string dataDir = Path.Combine(_tempWorkingDir, "DataStorage");
            Directory.CreateDirectory(dataDir);
            File.WriteAllText(Path.Combine(dataDir, "users.json"), "[]");
            File.WriteAllText(Path.Combine(dataDir, "products.json"), "[]");
            File.WriteAllText(Path.Combine(dataDir, "orders.json"), "[]");
            File.WriteAllText(Path.Combine(dataDir, "suppliers.json"), "[]");

            var data = new Data();
            await RunAsyncVoid(() => DataManager.UploadFilesFrom(data));

            Assert.DoesNotContain("Missing data files detected!", _testConsole.Output);
        }

        [TestMethod]
        public async Task UploadFilesFrom_RegistersDefaultErrorOutputInErrorRegistry() {
            string dataDir = Path.Combine(_tempWorkingDir, "DataStorage");
            Directory.CreateDirectory(dataDir);
            File.WriteAllText(Path.Combine(dataDir, "users.json"), "[]");
            File.WriteAllText(Path.Combine(dataDir, "products.json"), "[]");
            File.WriteAllText(Path.Combine(dataDir, "orders.json"), "[]");
            File.WriteAllText(Path.Combine(dataDir, "suppliers.json"), "[]");

            var data = new Data();
            await RunAsyncVoid(() => DataManager.UploadFilesFrom(data));

            ErrorRegistry.HandleError("Test registry error");

            Assert.Contains("Test registry error", _testConsole.Output);
        }

        #endregion

        #region SaveFilesFrom Tests

        [TestMethod]
        public async Task SaveFilesFrom_ValidData_SavesAllFilesAndCreatesBackupArchive() {
            var data = new Data();
            data.Users.Add(new User(1, "SavedUser", "pass1234"));

            await RunAsyncVoid(() => DataManager.SaveFilesFrom(data));

            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "users.json")));
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "products.json")));
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "orders.json")));
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "suppliers.json")));

            Assert.IsTrue(Directory.Exists("Backups"));
            string[] zipFiles = Directory.GetFiles("Backups", "*.zip");
            Assert.HasCount(1, zipFiles);
        }

        #endregion

        #region Async Void Synchronization Helper

        private static Task RunAsyncVoid(Action asyncVoidAction) {
            var previousContext = SynchronizationContext.Current;
            var trackingContext = new AsyncVoidTrackingContext();
            SynchronizationContext.SetSynchronizationContext(trackingContext);

            try {
                asyncVoidAction();
                return trackingContext.Completion;
            }
            finally {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        private sealed class AsyncVoidTrackingContext : SynchronizationContext {
            private int _operationCount;
            private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

            public Task Completion => _tcs.Task;

            public override void OperationStarted() {
                Interlocked.Increment(ref _operationCount);
            }

            public override void OperationCompleted() {
                if (Interlocked.Decrement(ref _operationCount) == 0) {
                    _tcs.TrySetResult(true);
                }
            }

            public override void Post(SendOrPostCallback d, object state) {
                OperationStarted();
                ThreadPool.QueueUserWorkItem(_ => {
                    try {
                        d(state);
                    }
                    catch (Exception ex) {
                        _tcs.TrySetException(ex);
                    }
                    finally {
                        OperationCompleted();
                    }
                });
            }
        }

        #endregion
    }
}
