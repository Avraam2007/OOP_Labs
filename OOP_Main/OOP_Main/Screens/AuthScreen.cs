using NOptional;
using OOP_Main.Utilities;
using System;

namespace OOP_Main.Screens {
    public class AuthScreen: BaseScreen, ICreateScreen {
        public AuthScreen(Data appData): base(appData) { }

        public string LoginScreen() {
            ShowHeader("Log in");

            string username = DefaultTextPrompt<string>("What's your [green]username[/]?");

            string password = PasswordPrompt("What's your [green]password[/]?");

            IOptional<User> foundUser = _appData.GetUserByUsername(username);

            if (foundUser.IsEmpty() || !UserService.CheckPassword(password, foundUser.Value) || foundUser.Value.IsGuest()) {
                DefaultErrorOutput($"Invalid username or password. Try again");

            }
            else {
                if (_appData.CurrentUser.IsEmpty() || _appData.CurrentUser != foundUser) {
                    _appData.CurrentUser = foundUser;
                }
                DefaultSuccessOutput($"Welcome back, {username}!");
            }
            return BackToMenuPrompt();
        }

        public string Create() {
            ShowHeader("Sign up");

            string username = DefaultTextPrompt<string>("Enter your [green]username[/]:");

            if (_appData.GetUserByUsername(username).HasValue()) {
                DefaultErrorOutput($"Sorry, this user was created earlier. You can log in to this account instead");
                return BackToMenuPrompt(
                    GetEnumDescription(MenuOption.LogIn)
                );
            }

            string password = PasswordPrompt("Create your [green]password[/]:");

            string confirmPassword = PasswordPrompt("Confirm your [green]password[/]:");

            if (confirmPassword == password) {
                try {
                    _appData.AddUser(username, password);

                    int createdUserId = _appData.GetUserByUsername(username).Value.Id;

                    _appData.CurrentUser = _appData.GetUserById(createdUserId);
                    DefaultSuccessOutput("Account was created!");
                }
                catch (ArgumentException e) {
                    DefaultErrorOutput(e.Message);
                    return BackToMenuPrompt();
                }
                return BackToMenuPrompt();
            }
            else {
                DefaultErrorOutput($"Incorrect password. Try again");
                return BackToMenuPrompt();
            }
        }
    }
}
