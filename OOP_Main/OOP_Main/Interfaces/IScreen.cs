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
    internal interface IEditScreen {
        string Edit();
    }
    internal interface IScreen : IShowScreen, ICreateScreen, IDeleteScreen, IEditScreen { }
}
