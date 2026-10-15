using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public struct FileService {
        private enum FileMeasureMode {
            Bytes,
            Lines
        }

        private static long GetFileMetrics(string filePath, FileMeasureMode mode) {
            if (!File.Exists(filePath)) return 0;

            switch (mode) {
                case FileMeasureMode.Bytes:
                    return new FileInfo(filePath).Length;

                case FileMeasureMode.Lines:
                    long lineCount = 0;

                    using (var reader = new StreamReader(filePath)) {
                        while (reader.ReadLine() != null) {
                            lineCount++;
                        }
                    }
                    return lineCount;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported measurement mode.");
            }
        }

        public static void EnsureDirectoryExists(string directoryPath) {
            if (!(string.IsNullOrEmpty(directoryPath) || Directory.Exists(directoryPath))) {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static long GetFileSizeInBytes(string filePath) => GetFileMetrics(filePath, FileMeasureMode.Bytes);

        public static int GetFileSizeInLines(string filePath) => (int)GetFileMetrics(filePath, FileMeasureMode.Lines);

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
