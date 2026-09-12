using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OOP_Main {
    internal class Program {
        static void Main(string[] args) {
            Interface consoleInterface = new Interface();
            consoleInterface.BootUpScreen();
        }
    }
}
