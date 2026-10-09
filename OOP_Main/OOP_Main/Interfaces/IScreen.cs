using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    internal interface IShowScreen {
        string Show();
    }
    internal interface ICreateScreen {
        string Create();
    }
    internal interface IDeleteScreen {
        string Delete();
    }
    internal interface IScreen : IShowScreen, ICreateScreen, IDeleteScreen { }
}
