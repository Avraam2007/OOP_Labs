using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Interfaces;
using OOP_Main.Repositories;
using OOP_Main.Utilities;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OOP_UnitTests.Utilities {
    [TestClass]
    public class AutoSaveServiceTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private AutoSaveService _service;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_AutoSaveTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            AnsiConsole.Console = _testConsole;

            _appData = new Data();
            _appData.Users.Add(new User(1, "TestUser", "pass1234"));
            _appData.Products.Add(new Cloth("CL-0001", "Shirt", 30.0, "Cotton", 1));
        }

        [TestCleanup]
        public void Cleanup() {
            _service?.Dispose();
            AnsiConsole.Console = _originalConsole;
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                for (int i = 0; i < 5; i++) {
                    try {
                        Directory.Delete(_tempWorkingDir, recursive: true);
                        break;
                    }
                    catch (IOException) when (i < 4) {
                        Thread.Sleep(50);
                    }
                }
            }
        }

        #region Lifecycle & Stop/Dispose Tests

        [TestMethod]
        public void StartAndDispose_StoppedBeforeIntervalElapses_DoesNotSaveFiles() {
            var userStorage = new FakeEntityStorage<User>();
            var productTextStorage = new FakeEntityStorage<Product>();
            var productBinaryStorage = new FakeEntityStorage<Product>();

            _service = new AutoSaveService(
                _appData,
                intervalSeconds: 5,
                userTextStorage: userStorage,
                productTextStorage: productTextStorage,
                productBinaryStorage: productBinaryStorage
            );

            _service.Start();
            Thread.Sleep(100);
            _service.Dispose();

            Assert.AreEqual(0, userStorage.SaveCount);
            Assert.AreEqual(0, productTextStorage.SaveCount);
            Assert.AreEqual(0, productBinaryStorage.SaveCount);
        }

        #endregion

        #region Background Save Execution Tests

        [TestMethod]
        public void Start_IntervalElapses_SavesUsersAndProductsToAllStorages() {
            using (var savedEvent = new ManualResetEventSlim(false)) {

                var userStorage = new FakeEntityStorage<User>();
                var productTextStorage = new FakeEntityStorage<Product>();
                var productBinaryStorage = new FakeEntityStorage<Product>(onSave: () => savedEvent.Set());

                _service = new AutoSaveService(
                    _appData,
                    intervalSeconds: 1,
                    userTextStorage: userStorage,
                    productTextStorage: productTextStorage,
                    productBinaryStorage: productBinaryStorage
                );

                _service.Start();

                bool isSaved = savedEvent.Wait(TimeSpan.FromSeconds(4), TestContext.CancellationToken);
                _service.Stop();

                Assert.IsTrue(isSaved, "Фоновий потік не виконав збереження в межах очікуваного часу.");
                Assert.IsGreaterThanOrEqualTo(1, userStorage.SaveCount);
                Assert.AreEqual("DataStorage/text/users.txt", userStorage.LastSavedPath);
                Assert.AreSame(_appData.Users, userStorage.LastSavedItems);

                Assert.IsGreaterThanOrEqualTo(1, productTextStorage.SaveCount);
                Assert.AreEqual("DataStorage/text/products.txt", productTextStorage.LastSavedPath);

                Assert.IsGreaterThanOrEqualTo(1, productBinaryStorage.SaveCount);
                Assert.AreEqual("DataStorage/bin/products.dat", productBinaryStorage.LastSavedPath);
            }
        }

        [TestMethod]
        public void RunAutoSave_StorageThrowsException_CatchesErrorWithoutCrashingBackgroundThread() {
            using (var errorTriggeredEvent = new ManualResetEventSlim(false)) {

                var failingUserStorage = new FakeEntityStorage<User>(onSave: () => {
                    errorTriggeredEvent.Set();
                    throw new IOException("Simulated disk write error");
                });

                _service = new AutoSaveService(
                    _appData,
                    intervalSeconds: 1,
                    userTextStorage: failingUserStorage,
                    productTextStorage: new FakeEntityStorage<Product>(),
                    productBinaryStorage: new FakeEntityStorage<Product>()
                );

                _service.Start();

                bool exceptionHandled = errorTriggeredEvent.Wait(TimeSpan.FromSeconds(4), TestContext.CancellationToken);
                Thread.Sleep(100);
                _service.Stop();

                Assert.IsTrue(exceptionHandled, "Очікуваний виклик сховища не відбувся.");
                Assert.IsGreaterThanOrEqualTo(1, failingUserStorage.SaveCount);
            }
        }

        #endregion

        #region Test Doubles

        private class FakeEntityStorage<T> : IEntityStorage<T> {
            private readonly Action _onSave;
            private int _saveCount;

            public int SaveCount => Volatile.Read(ref _saveCount);
            public string LastSavedPath { get; private set; }
            public List<T> LastSavedItems { get; private set; }

            public FakeEntityStorage(Action onSave = null) {
                _onSave = onSave;
            }

            public List<T> LoadFromFile(string filePath) => new List<T>();

            public void SaveToFile(string filePath, List<T> items) {
                LastSavedPath = filePath;
                LastSavedItems = items;
                Interlocked.Increment(ref _saveCount);
                _onSave?.Invoke();
            }
        }

        public TestContext TestContext { get; set; }

        #endregion
    }
}
