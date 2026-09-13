using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OOP_Main {
    public class Interface {
        private Data appData;

        public Data AppData { get => appData; set => appData = value; }

        public Interface(Data appData) {
            this.AppData = appData;
        }
        public void StatusSpinner(string text) {
            AnsiConsole.Status()
                .Start(text, ctx => {
                    Thread.Sleep(2400);
                });
        }
        public void BootUpScreen() {
            // Styled text with markup
            AnsiConsole.MarkupLine("[bold blue]ECommerce[/] [green]v0.7[/]");

            // Status spinner for work
            StatusSpinner("Loading...");
            MainScreen();
        }

        public void BootDownScreen() {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Are you sure you want to quit the app?")
                    .AddCancelResult("No")
                    .DefaultValue("No")
                    .AddChoices("Yes", "No"));

            switch (choice) {
                case "Yes":
                    StatusSpinner("Shutting down...");
                    break;
                case "No":
                    MainScreen();
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
            
        }

        public void ShowHeader(string text) {
            var panel = new Panel(
                new Markup(text, new Style(foreground: Color.Blue))
                )
                .BorderColor(Color.Blue)
                .Padding(2, 2);
            var padder = new Padder(panel);

            AnsiConsole.Write(padder);
            AnsiConsole.Write(new Rule());
        }

        public void CreateUserScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Sign up[/]");
            var usernamePrompt = new TextPrompt<string>("Enter your [green]username[/]:");

            string username = AnsiConsole.Prompt(usernamePrompt);

            var passwordPrompt = new TextPrompt<string>("Enter your [green]password[/]:")
                .Secret();

            string password = AnsiConsole.Prompt(passwordPrompt);

            var confirmPasswordPrompt = new TextPrompt<string>("Confirm your [green]password[/]:")
                .Secret();

            string confirmPassword = AnsiConsole.Prompt(confirmPasswordPrompt);

            if (confirmPassword == password) {
                AppData.CurrentUser = new User("258", username, password);
                AppData.AddUser(AppData.CurrentUser);
                AnsiConsole.MarkupLine($"[green bold]Account was created![/]");
                BackToMenu();
            }
        }

        public void LoginScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Log in[/]");
            var usernamePrompt = new TextPrompt<string>("What's your [green]username[/]?");

            string username = AnsiConsole.Prompt(usernamePrompt);

            if (AppData.GetUserByUsername(username) == null) {
                AnsiConsole.MarkupLine($"[red bold]Sorry, we didn't found this account. Try again or sign in this account[/]");
                BackToMenu("Sign up");
            }

            var passwordPrompt = new TextPrompt<string>("What's your [green]password[/]?")
                .Secret();

            string password = AnsiConsole.Prompt(passwordPrompt);

            if (AppData.GetUserByUsername(username).Password != password ) {
                AnsiConsole.MarkupLine($"[red bold]Invalid username or password. Try again[/]");
                BackToMenu();
            }
            else {
                if (AppData.CurrentUser == null) {
                    AppData.CurrentUser = AppData.GetUserByUsername(username);
                }
                AnsiConsole.MarkupLine($"[green bold]Welcome back, {username}![/]");
            }
        }

        public void BackToMenu(string extraOption = "") {
            List<string> options = new List<string> { "Back to menu" };
            if (extraOption != "") {
                options.Add(extraOption);
            }
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Press ESC or Enter to back to menu{(extraOption != "" ? $" or choose {extraOption} option" : "")}")
                    .AddCancelResult("Back to menu")
                    .DefaultValue("Back to menu")
                    .AddChoices(options));
            switch (choice) {
                case "Back to menu":
                    MainScreen();
                    break;
                case "Sign up":
                    CreateUserScreen();
                    break;
                case "Log in":
                    LoginScreen();
                    break;
                default:
                    break;
            }
        }

        public void MainScreen() {
            AnsiConsole.Clear();

            ShowHeader("[bold]Welcome to [green]ECommerce[/][/]");
            List<string> menuList = new List<string>{"Show users",
                        "Show orders",
                        "Show products",
                        "Create order",
                        "Add product",
                        "Sign up",
                        "Log in",
                        "Quit"
            };
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose option")
                    .WrapAround()
                    .AddChoices(menuList));
            if (choice == "Quit") {
                BootDownScreen();
            }
            if (choice == "Sign up") {
                CreateUserScreen();
            }
            if (choice == "Log in") {
                LoginScreen();
                BackToMenu();
            }
            if (choice == "Create order") {
                if (AppData.CurrentUser == null) {
                    AnsiConsole.MarkupLine($"[red bold]The access is forbidden[/]");
                    BackToMenu("Sign up");
                }
                else {
                    AnsiConsole.MarkupLine($"[red bold]You should log in first[/]");
                    BackToMenu("Log in");
                }
            }
            if ((choice == "Add product" || choice == "Show users")) {
                if (AppData.CurrentUser != null && !AppData.CurrentUser.IsAdmin) {
                    AnsiConsole.MarkupLine($"[red bold]You don't have access. Only for admin[/]");
                    BackToMenu();
                }
                else {
                    AnsiConsole.MarkupLine($"[red bold]The access is forbidden. Only for admin[/]");
                    BackToMenu();
                }
            }
        }
    }
}
