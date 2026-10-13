using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public struct FileService {
        public static void EnsureDirectoryExists(string directoryPath) {
            if (!(string.IsNullOrEmpty(directoryPath) || Directory.Exists(directoryPath))) {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static long GetFileSizeInBytes(string filePath) {
            if (!File.Exists(filePath)) return 0;
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public static int DeleteOldFiles(string folderPath, string searchPattern, int maxFilesToKeep) {
            if (!Directory.Exists(folderPath)) return 0;

            var dirInfo = new DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles(searchPattern)
                               .OrderByDescending(f => f.CreationTime)
                               .ToList();

            int deletedCount = 0;
            if (files.Count > maxFilesToKeep) {
                var filesToDelete = files.Skip(maxFilesToKeep);
                foreach (var file in filesToDelete) {
                    file.Delete();
                    deletedCount++;
                }
            }
            return deletedCount;
        }
    }
}
