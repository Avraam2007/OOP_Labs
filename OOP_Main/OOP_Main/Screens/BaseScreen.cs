using OOP_Main.UI;
using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main.Screens {
    public abstract class BaseScreen: UIHelper {
        protected readonly Data _appData;
        public BaseScreen(Data appData) {
            _appData = appData;
        }

        public string EditObjectScreen<T>(List<T> items) 
            where T : class {
            string typeName = typeof(T).Name.ToLowerInvariant();
            ShowHeader($"Edit {typeName}");

            List<T> finalList = new List<T>(items);

            if (items is List<User> users) {
                var filteredUsers = users.Where(user => !(CheckIfUserIsAdmin(user) || user.IsGuest())).ToList();
                finalList = filteredUsers as List<T> ?? new List<T>();
            }

            if (Tools.ValidateArray(finalList)) {
                DefaultErrorOutput($"There are no {typeName}s found.");
                return BackToMenuPrompt();
            }

            var entityChoice = AnsiConsole.Prompt(
                new SelectionPrompt<T>()
                    .Title($"Select {typeName} to edit:")
                    .UseConverter(x => x.ToString())
                    .AddChoices(finalList)
            );

            EntityEditorEngine.EditEntity(entityChoice);

            _appData.SaveAllData();

            return BackToMenuPrompt();
        }

        protected string DeleteObjectScreen<T, TId>(List<T> items, Func<TId, bool> deleteAction, Func<T, string> nameSelector)
            where T : class, IIdentifiable<TId>
            where TId : IEquatable<TId> {
            string typeName = typeof(T).Name.ToLowerInvariant();

            ShowHeader($"Delete {typeName}");

            List<T> finalList = new List<T>(items);

            if (items is List<User> users) {
                var filteredUsers = users.Where(user => !(CheckIfUserIsAdmin(user) || user.IsGuest())).ToList();
                finalList = filteredUsers as List<T> ?? new List<T>();
            }

            if (Tools.ValidateArray(finalList)) {
                DefaultErrorOutput($"There are no {typeName}s to delete.");
                return BackToMenuPrompt();
            }

            var itemChoicesMap = finalList
                .GroupBy(item => Markup.Escape(item.ToString()))
                .ToDictionary(g => g.Key, g => g.First());

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

        private readonly Action<int, int> PageCheck = (currentPage, totalPages) => {
            DefaultMarkupOutput($"Page {currentPage} of {totalPages}");
        };

        protected string ShowListScreen<T>(string header, IEnumerable<T> items, string extraErrorMessage = "") where T : class {
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
