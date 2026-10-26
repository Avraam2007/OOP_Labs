using OOP_Main.Entities;
using OOP_Main.FileIO;
using OOP_Main.FileIO.BinaryEntities;
using OOP_Main.FileIO.TextEntities;
using OOP_Main.Interfaces;
using OOP_Main.Repositories;
using OOP_Main.UI;
using System;
using System.Threading;

namespace OOP_Main.Utilities {
    public class AutoSaveService : IDisposable {
        private readonly Data _appData;
        private readonly Thread _workerThread;
        private readonly object _lockObj = new object();
        private bool _isRunning = true;
        private readonly int _intervalSeconds;

        private readonly IEntityStorage<User> _userTextStorage;
        private readonly IEntityStorage<Product> _productTextStorage;
        private readonly IEntityStorage<Product> _productBinaryStorage;

        public AutoSaveService(
            Data appData, 
            int intervalSeconds = 30,
            IEntityStorage<User> userTextStorage = null,
            IEntityStorage<Product> productTextStorage = null,
            IEntityStorage<Product> productBinaryStorage = null
        ) {
            _appData = appData;
            _userTextStorage = userTextStorage ?? new TextUserStorage();
            _productTextStorage = productTextStorage ?? new TextProductStorage();
            _productBinaryStorage = productBinaryStorage ?? new BinaryProductStorage();
            _intervalSeconds = intervalSeconds;

            _workerThread = new Thread(RunAutoSave) {
                IsBackground = true,
                Name = "AutoSaveThread"
            };
        }

        public void Start() {
            _workerThread.Start();
        }

        private void RunAutoSave() {
            while (_isRunning) {
                try {
                    Thread.Sleep(_intervalSeconds * 1000);

                    if (!_isRunning) break;

                    lock (_lockObj) {
                        _userTextStorage.SaveToFile("DataStorage/text/users.txt", _appData.Users);
                        _productTextStorage.SaveToFile("DataStorage/text/products.txt", _appData.Products);
                        _productBinaryStorage.SaveToFile("DataStorage/bin/products.dat", _appData.Products);
                        DataManager.SaveFilesFrom(_appData);
                        Logger.Log("Data successfully saved in a background thread", Logger.LogLevels.Info);
                    }
                }
                catch (Exception ex) {
                    Logger.Log($"Background save error: {ex.Message}", Logger.LogLevels.Error);
                }
            }
        }

        public void Stop() {
            _isRunning = false;
        }

        public void Dispose() {
            Stop();
        }
    }
}
