using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main {
    public class BackupService {
        public static string CreateZipBackup(string sourceDirectory, string backupTargetFolder) {
            try {
                FileService.EnsureDirectoryExists(sourceDirectory);

                FileService.EnsureDirectoryExists(backupTargetFolder);

                string timeStamp = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                string zipFileName = Path.Combine(backupTargetFolder, $"backup_{timeStamp}.zip");

                if (File.Exists(zipFileName)) File.Delete(zipFileName);

                ZipFile.CreateFromDirectory(sourceDirectory, zipFileName);

                FileService.DeleteOldFiles(backupTargetFolder, "*.zip", maxFilesToKeep: 5);

                return zipFileName;
            }
            catch (Exception ex) {
                throw new ECommerceException($"Error with creating archive: {ex.Message}");
            }
        }

        public static void RestoreFromZipBackup(string zipFilePath, string destinationDirectory) {
            if (!File.Exists(zipFilePath)) {
                throw new FileNotFoundException($"Archive {zipFilePath} not found.");
            }

            if (Directory.Exists(destinationDirectory)) {
                Directory.Delete(destinationDirectory, true);
            }

            ZipFile.ExtractToDirectory(zipFilePath, destinationDirectory);
        }

        public static string GetLatestBackupFilePath(string backupFolder) {
            if (!Directory.Exists(backupFolder)) return null;

            DirectoryInfo dir = new DirectoryInfo(backupFolder);
            FileInfo latestFile = dir.GetFiles("*.zip")
                                     .OrderByDescending(f => f.CreationTime)
                                     .FirstOrDefault();

            return latestFile?.FullName;
        }
    }
}
