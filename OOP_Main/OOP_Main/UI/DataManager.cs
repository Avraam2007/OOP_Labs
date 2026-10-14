using System;
using System.IO;
using System.Linq;

namespace OOP_Main.UI {
    internal class DataManager: UIHelper {
        private static void EnsureDataOrRestoreBackup() {
            string dataFolder = "DataStorage";
            string backupFolder = "Backups";

            string[] requiredFiles = new[] {
                Path.Combine(dataFolder, "users.json"),
                Path.Combine(dataFolder, "products.json"),
                Path.Combine(dataFolder, "orders.json"),
                Path.Combine(dataFolder, "suppliers.json")
            };

            bool missingAnyFile = requiredFiles.Any(filePath => !File.Exists(filePath));

            if (missingAnyFile) {
                string latestBackup = BackupService.GetLatestBackupFilePath(backupFolder);

                if (!string.IsNullOrEmpty(latestBackup)) {
                    DefaultMarkupOutput("[yellow bold]Missing data files detected! Restoring from archive...[/]");

                    try {
                        BackupService.RestoreFromZipBackup(latestBackup, dataFolder);
                        DefaultSuccessOutput($"Data is restored successfully: [cyan]{Path.GetFileName(latestBackup)}[/]");
                    }
                    catch (Exception ex) {
                        DefaultErrorOutput($"Failed to restore from the archive: {ex.Message}");
                    }
                }
                else {
                    DefaultMarkupOutput("[yellow]Data files are missing, and no backups were found in the Backups folder. New files will be created.[/]");
                }
            }
        }

        public static void SaveFilesFrom(Data data) {
            data.SaveAllData();
            data.SaveBackup();
        }

        public static void UploadFilesFrom(Data data) {
            JsonStorage.RegisterHandler(DefaultErrorOutput);
            TextStorage.RegisterHandler(DefaultErrorOutput);
            EnsureDataOrRestoreBackup();
            data.LoadAllData();
        }
    }
}
