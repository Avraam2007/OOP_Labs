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
        protected static void StatusSpinner(string text, Action actionDuringLoading = null, int loadingTimeInMs = 2400) {
            IOptional<Action> DoSmth = Optional.OfNullable(actionDuringLoading);
            AnsiConsole.Status()
                .Start(text, ctx => {
                    DoSmth.GetValueOrElse(() => { })();
                    Thread.Sleep(loadingTimeInMs);
                });
        }

        protected static T DefaultTextPrompt<T>(string text) {
            var prompt = new TextPrompt<T>(text);
            T value = AnsiConsole.Prompt(prompt);

            return value is string val ? (T)(object)Markup.Escape(val) : value;
        }

        public static void DefaultMarkupOutput(string text) {
            AnsiConsole.MarkupLine(text);
        }

        public static void DefaultErrorOutput(string message) {
            DefaultMarkupOutput($"[red bold]{message}[/]");
        }

        public static void DefaultSuccessOutput(string message) {
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

        public string DeleteObjectScreen<T, TId>(List<T> items, Func<TId, bool> deleteAction, Func<T, string> nameSelector) 
            where T : class, IIdentifiable<TId>
            where TId : IEquatable<TId> {
            string typeName = typeof(T).Name.ToLowerInvariant();
            ClearConsole();
            ShowHeader($"Delete {typeName}");

            List<T> finalList = new List<T>(items);

            if(items is List<User> users) {
                var filteredUsers = users.Where(user => !(CheckIfUserIsAdmin(user) || user.IsGuest())).ToList();
                finalList = filteredUsers as List<T> ?? new List<T>();
            }

            if (Tools.ValidateArray(finalList)) {
                DefaultErrorOutput($"There are no {typeName}s to delete.");
                return BackToMenuPrompt();
            }

            var itemChoicesMap = new Dictionary<string, T>();
            foreach (var item in finalList) {
                string key = Markup.Escape(item.ToString());
                if (!itemChoicesMap.ContainsKey(key)) {
                    itemChoicesMap.Add(key, item);
                }
            }

            var choices = itemChoicesMap.Keys.ToList();

            choices.Add("Cancel");

            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Select the {typeName} to delete:")
                    .PageSize(10)
                    .WrapAround()
                    .AddChoices(choices));

            if (selectedChoice == "Cancel") return BackToMenuPrompt();

            T itemToDelete = itemChoicesMap[selectedChoice];

            if (itemToDelete == null) {
                DefaultErrorOutput($"This {typeName} is not found.");
                return BackToMenuPrompt();
            }

            string displayName = nameSelector(itemToDelete);

            bool confirm = DefaultConfirm(
                $"Are you sure you want to delete {displayName}?",
                defaultValue: false
            );

            if (confirm) {
                bool isDeleted = deleteAction(itemToDelete.Id);

                if (isDeleted) {
                    DefaultSuccessOutput($"{typeof(T).Name} successfully deleted!");
                }
                else {
                    DefaultErrorOutput($"Failed to delete {typeName}.");
                }
            }
            else {
                DefaultMarkupOutput("[yellow]Deleting cancelled.[/]");
            }

            return BackToMenuPrompt();
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
