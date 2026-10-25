using Spectre.Console;
using System;
using OOP_Main.UI;
using OOP_Main.Utilities;
using OOP_Main.Repositories;

namespace OOP_Main {
    internal class Program {
        static void Main(string[] args) {
            try {
                Data appData = new Data();
                Interface consoleInterface = new Interface(appData);
                Logger.Log("The program work is started", Logger.LogLevels.Info);
                consoleInterface.Start();
            }
            catch (Exception ex) {
                Logger.Log($"Fatal error: {ex}", Logger.LogLevels.Error);
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                Console.ReadKey();
            }
        }
    }
}
