using NOptional;
using Spectre.Console;
using System;

namespace OOP_Main {
    public class AuthScreen: BaseScreen {
        public AuthScreen(Data appData): base(appData) { }

        public string LoginScreen() {
            ClearConsole();
            ShowHeader("[bold]Log in[/]");

            string username = DefaultTextPrompt<string>("What's your [green]username[/]?");

            string password = PasswordPrompt("What's your [green]password[/]?");

            IOptional<User> foundUser = _appData.GetUserByUsername(username);

            if (foundUser.IsEmpty() || foundUser.Value.Password != password || foundUser.Value.IsGuest()) {
                DefaultMarkupOutput($"[red bold]Invalid username or password. Try again[/]");
                return BackToMenuPrompt();

            }
            else {
                User user = foundUser.Value;
                if (_appData.CurrentUser == null || _appData.CurrentUser != user) {
                    _appData.CurrentUser = user;
                }
                DefaultMarkupOutput($"[green bold]Welcome back, {username}![/]");
            }
            return BackToMenuPrompt();
        }

        public override string Create() {
            ClearConsole();
            ShowHeader("[bold]Sign up[/]");

            string username = DefaultTextPrompt<string>("Enter your [green]username[/]:");

            if (_appData.GetUserByUsername(username).HasValue()) {
                DefaultMarkupOutput($"[red bold]Sorry, this account was created earlier. You can log in to this account instead[/]");
                return BackToMenuPrompt(
                    GetEnumDescription(MenuOption.LogIn)
                );
            }

            string password = PasswordPrompt("Create your [green]password[/]:");

            string confirmPassword = PasswordPrompt("Confirm your [green]password[/]:");

            if (confirmPassword == password) {
                _appData.AddUser(username, password);

                int createdUserId = _appData.GetUserByUsername(username).Value.Id;

                _appData.CurrentUser = _appData.GetUserById(createdUserId).Value;
                DefaultMarkupOutput($"[green bold]Account was created![/]");
                return BackToMenuPrompt();
            }
            else {
                DefaultMarkupOutput($"[red bold]Incorrect password. Try again[/]");
                return BackToMenuPrompt();
            }
        }

        public override string Show() {
            throw new NotImplementedException();
        }

        public override string Delete() {
            throw new NotImplementedException();
        }
    }
}
