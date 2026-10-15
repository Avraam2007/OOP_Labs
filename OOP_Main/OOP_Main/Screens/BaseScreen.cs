using OOP_Main.UI;

namespace OOP_Main.Screens {
    public abstract class BaseScreen: UIHelper {
        protected readonly Data _appData;
        public BaseScreen(Data appData) {
            _appData = appData;
        }
    }
}
