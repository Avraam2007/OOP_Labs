namespace OOP_Main {
    public abstract class BaseScreen: UIHelper {
        protected readonly Data _appData;
        public BaseScreen(Data appData) {
            _appData = appData;
        }
    }
}
