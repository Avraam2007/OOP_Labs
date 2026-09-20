namespace OOP_Main {
    public class UserScreen: UIHelper {
        private readonly Data _appData;
        public UserScreen(Data appData) {
            _appData = appData;
        }

        public string ShowUsersScreen() => ShowListScreen("Users", _appData.Users);
    }
}
