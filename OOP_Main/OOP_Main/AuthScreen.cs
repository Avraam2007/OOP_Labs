using Spectre.Console;
using System;

namespace OOP_Main {
    public class AuthScreen: UIHelper {
        private readonly Data _appData;
        public AuthScreen(Data appData) {
            _appData = appData;
        }

        public string LoginScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Log in[/]");

            string username = DefaultTextPrompt<string>("What's your [green]username[/]?");

            string password = PasswordPrompt("What's your [green]password[/]?");

            User user = _appData.GetUserByUsername(username);

            if (user == null || user.Password != password) {
                AnsiConsole.MarkupLine($"[red bold]Invalid username or password. Try again[/]");
                return BackToMenuPrompt();

            }
            else {
                if (_appData.CurrentUser == null || _appData.CurrentUser != user) {
                    _appData.CurrentUser = user;
                }
                AnsiConsole.MarkupLine($"[green bold]Welcome back, {username}![/]");
            }
            return BackToMenuPrompt();
        }

        public string CreateUserScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Sign up[/]");

            string username = DefaultTextPrompt<string>("Enter your [green]username[/]:");

            if (_appData.GetUserByUsername(username) != null) {
                AnsiConsole.MarkupLine($"[red bold]Sorry, this account was created earlier. You can log in to this account instead[/]");
                return BackToMenuPrompt(
                    GetEnumDescription(MenuOption.LogIn)
                );
            }

            string password = PasswordPrompt("Create your [green]password[/]:");

            string confirmPassword = PasswordPrompt("Confirm your [green]password[/]:");

            if (confirmPassword == password) {
                string newId = new Random().Next(100, 999).ToString();
                _appData.CurrentUser = new User(newId, username, password);
                _appData.AddUser(_appData.CurrentUser);
                AnsiConsole.MarkupLine($"[green bold]Account was created![/]");
                return BackToMenuPrompt();
            }
            else {
                AnsiConsole.MarkupLine($"[red bold]Incorrect password. Try again[/]");
                return BackToMenuPrompt();
            }
        }
    }
}
