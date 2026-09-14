using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Main {
    public interface IDataAndUIBridge {
        Dictionary<string, Text> ShowInfo();
    }
}
