using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OOP_Main {
    public class UIHelper {
        protected string GetEnumDescription(Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null) return value.ToString();
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        protected static void StatusSpinner(string text, int loadingTimeInMs = 2400) {
            AnsiConsole.Status()
                .Start(text, ctx => {
                    Thread.Sleep(loadingTimeInMs);
                });
        }

        protected static T DefaultTextPrompt<T>(string text) {
            var prompt = new TextPrompt<T>(text);

            return AnsiConsole.Prompt(prompt);
        }

        protected static string PasswordPrompt(string text) {
            var passwordPrompt = new TextPrompt<string>("What's your [green]password[/]?")
                .Secret();

            return AnsiConsole.Prompt(passwordPrompt);
        }

        protected static void InternalFlashCard(Dictionary<string, Spectre.Console.Text> dataForCard) {
            var contentElements = new List<IRenderable>();
            foreach (var element in dataForCard) {
                contentElements.Add(element.Value);
            }
            var content = new Rows(contentElements);
            var panel = new Panel(content)
            .BorderColor(Color.Blue)
            .Padding(4, 2);

            AnsiConsole.Write(panel);
        }

        protected static void ShowHeader(string text) {
            var logoPanel = new Panel(
                new Markup(text, new Style(foreground: Color.Blue))
                )
                .BorderColor(Color.Blue)
                .Padding(2, 2);
            var header = new Padder(logoPanel);

            AnsiConsole.Write(header);
            AnsiConsole.Write(new Rule());
        }

        protected static bool CheckIfUserIsAdmin(User currentUser) {
            return currentUser != null && currentUser.IsAdmin;
        }

        protected static string BackToMenuPrompt(string extraOption = "") {
            List<string> options = new List<string> { "Back to menu" };
            if (!string.IsNullOrWhiteSpace(extraOption)) {
                options.Add(extraOption);
            }
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"\nPress ESC or Enter to back to menu{(extraOption != "" ? $" or choose {extraOption} option" : "")}")
                    .AddCancelResult("Back to menu")
                    .DefaultValue("Back to menu")
                    .AddChoices(options));

            return choice;
        }

        protected string ShowListScreen<T>(string header, IEnumerable<T> items, string extraErrorMessage = "") where T : class {
            AnsiConsole.Clear();
            ShowHeader(header);

            if (items == null || !items.Any()) {
                AnsiConsole.Markup($"[red bold]Sorry, we didn't find any {header.ToLower()} in the store. {extraErrorMessage}[/]");
            }
            else {
                foreach (var item in items) {
                    var dataForCard = CardRenderer.GetCardInfo(item);
                    InternalFlashCard(dataForCard);
                }
            }

                return BackToMenuPrompt();
        }
    }
}
