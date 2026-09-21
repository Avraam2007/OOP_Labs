namespace OOP_Main {
    public abstract class BaseScreen: UIHelper, IScreen {
        protected readonly Data _appData;
        public BaseScreen(Data appData) {
            _appData = appData;
        }

        public abstract string Show();

        public abstract string Create();

        public abstract string Delete();
    }
}
