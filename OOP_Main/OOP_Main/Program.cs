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
            User admin = new User("123", "Admin", "admin", true);
            Data appData = new Data();
            appData.AddUser(admin);
            Interface consoleInterface = new Interface(appData);
            consoleInterface.BootUpScreen();
        }
    }
}
