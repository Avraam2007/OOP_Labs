using Spectre.Console;
using System.Collections.Generic;
using System.Threading;

namespace OOP_Main {
    public class Interface {
        public void BootUpScreen() {
            // Styled text with markup
            AnsiConsole.MarkupLine("[bold blue]ECommerce[/] [green]v0.5[/]");

            // Status spinner for work
            AnsiConsole.Status()
                .Start("Loading...", ctx => {
                    Thread.Sleep(2400);
                });
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

            if (choice == "Yes") {
                AnsiConsole.Status()
                    .Start("Shutting down...", ctx => {
                        Thread.Sleep(1200);
                    });
            }
            if (choice == "No") {
                MainScreen();
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
            ShowHeader("[bold]Sign in[/]");
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
            if (choice == "Back to menu") {
                MainScreen();
            }
            if (choice == "Sign in") {
                CreateUserScreen();
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
                        "Sign in",
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
            if (choice == "Sign in") {
                CreateUserScreen();
            }
            if ((choice == "Add product" || choice == "Create order" || choice == "Show users")) {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden[/]");
                BackToMenu("Sign in");
            }
            else {
                AnsiConsole.MarkupLine($"You selected: [green]{choice}[/]");
            }
        }
    }
}
