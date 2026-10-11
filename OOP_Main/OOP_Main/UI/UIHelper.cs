using NOptional;
using OOP_Main.Utilities;
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
        public delegate string ConsolePromptHandler(string text); // Func<string, string>
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
            T value = AnsiConsole.Prompt(prompt);

            return value is string val ? (T)(object)Markup.Escape(val) : value;
        }

        protected static void DefaultMarkupOutput(string text) {
            AnsiConsole.MarkupLine(text);
        }

        protected static void DefaultErrorOutput(string message) {
            DefaultMarkupOutput($"[red bold]{message}[/]");
        }

        protected static void DefaultSuccessOutput(string message) {
            DefaultMarkupOutput($"[green bold]{message}[/]");
        }

        protected static Action ClearConsole = () => {
            AnsiConsole.Clear();
        };

        protected static ConsolePromptHandler PasswordPrompt = text => {
            var passwordPrompt = new TextPrompt<string>(text)
                .Secret();

            return AnsiConsole.Prompt(passwordPrompt);
        };

        protected static void InternalFlashCard(Dictionary<string, IRenderable> dataForCard) {
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

        protected static Action<string> ShowHeader = text => {
            var logoPanel = new Panel(
                new Markup($"[bold]{text}[/]", new Style(foreground: Color.Blue))
                )
                .BorderColor(Color.Blue)
                .Padding(2, 2);
            var header = new Padder(logoPanel);

            AnsiConsole.Write(header);
            AnsiConsole.Write(new Rule());
        };

        protected static bool CheckIfUserIsAdmin(IOptional<User> currentUser) {
            return currentUser.HasValue() && currentUser.Value.IsAdmin();
        }
        protected static bool CheckIfUserIsAdmin(User currentUser) => CheckIfUserIsAdmin(Optional.OfNullable(currentUser));

        protected static string BackToMenuPrompt(string extraOption = "") {
            List<string> options = new List<string> { "Back to menu" };
            if (!string.IsNullOrWhiteSpace(extraOption)) {
                options.Add(extraOption);
            }
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"\nPress ESC or Enter to back to menu{(extraOption != "" ? $" or choose \"{extraOption}\" option" : "")}")
                    .AddCancelResult("Back to menu")
                    .DefaultValue("Back to menu")
                    .AddChoices(options));

            return choice;
        }
        private readonly Action<int, int> PageCheck = (currentPage, totalPages) => {
            DefaultMarkupOutput($"Page {currentPage} of {totalPages}");
        };

        protected bool DefaultConfirm(string text, bool defaultValue = true) {
            return AnsiConsole.Confirm(text, defaultValue);
        }

        protected string ShowListScreen<T>(string header, IEnumerable<T> items, string extraErrorMessage = "") where T : class {
            ClearConsole();
            ShowHeader(header);

            if (Tools.ValidateArray(items)) {
                DefaultErrorOutput($"Sorry, we didn't find any {header.ToLower()} in the store. {extraErrorMessage}");
                return BackToMenuPrompt();
            }
            else {
                int pageSize = 3;
                int totalItems = items.Count();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                int currentPage = 1;

                while (true) {
                    ClearConsole();
                    ShowHeader(header);
                    PageCheck(currentPage, totalPages);

                    var pageItems = items.Skip((currentPage - 1) * pageSize).Take(pageSize);

                    foreach (var item in pageItems) {
                        InternalFlashCard(CardRenderer.GetCardInfo(item));
                    }

                    PageCheck(currentPage, totalPages);

                    var navigationOptions = new List<string>();
                    if (currentPage > 1) navigationOptions.Add("Previous Page");
                    if (currentPage < totalPages) navigationOptions.Add("Next Page");
                    navigationOptions.Add("Back to menu");

                    var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("\n[green]Navigation:[/]")
                            .WrapAround()
                            .AddChoices(navigationOptions));

                    if (choice == "Next Page") {
                        currentPage++;
                    }
                    else if (choice == "Previous Page") {
                        currentPage--;
                    }
                    else {
                        return choice;
                    }
                }
            }
        }
    }
}
