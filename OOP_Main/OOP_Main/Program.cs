using Spectre.Console;
using System;
using System.Collections.Generic;
using OOP_Main;

namespace OOP_Main {
    internal class Program {
        static void Main(string[] args) {
            try {
                Data appData = new Data();
                Interface consoleInterface = new Interface(appData);
                consoleInterface.Start();
            }
            catch (Exception ex) {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                Console.ReadKey();
            }
        }
    }
}
