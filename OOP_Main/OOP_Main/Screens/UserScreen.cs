using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OOP_Main {
    public class UserScreen: BaseScreen, IShowScreen, IDeleteScreen {
        public UserScreen(Data appData): base(appData) { }

        public string Show() => ShowListScreen("Users", _appData.Users);

        public string Delete() {
            ClearConsole();
            ShowHeader("Delete user");

            var regularUsers = new List<User>();

            _appData.Users.ForEach(user => { if (!(CheckIfUserIsAdmin(user) || user.IsGuest())) regularUsers.Add(user); });


            if (Tools.ValidateArray(regularUsers)) {
                DefaultErrorOutput("There are no users to delete.");
                return BackToMenuPrompt();
            }

            var userChoicesMap = new Dictionary<string, User>();
            foreach (var user in regularUsers) {
                userChoicesMap.Add(user.ToString(), user);
            }

            var choices = userChoicesMap.Keys.ToList();

            choices.Add("Cancel");

            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an user to delete:")
                    .PageSize(10)
                    .AddChoices(choices));

            if (selectedChoice == "Cancel") return BackToMenuPrompt();

            User userToDelete = userChoicesMap[selectedChoice];

            if (userToDelete != null) {
                bool confirm = AnsiConsole.Confirm(
                    $"Are you sure you want to delete [red]\"{userToDelete.Username}\"[/] ({userToDelete.Id})?",
                    defaultValue: false
                );

                if (confirm) {
                    bool isDeleted = _appData.DeleteUserById(userToDelete.Id);
                    if (isDeleted) {
                        DefaultMarkupOutput("[green]User successfully deleted![/]");
                    }
                    else {
                        DefaultErrorOutput("Failed to delete user.");
                    }
                }
                else {
                    DefaultMarkupOutput("[yellow]Deleting cancelled.[/]");
                }
            }

            return BackToMenuPrompt();
        }

        public string CheckAccount() {
            ClearConsole();
            ShowHeader("My account");

            InternalFlashCard(CardRenderer.GetCardInfo(_appData.CurrentUser.Value));
            //DefaultMarkupOutput($"Your name: {_appData.CurrentUser.Username}");
            //DefaultMarkupOutput($"Your status: {_appData.CurrentUser.Role}");
            //DefaultMarkupOutput($"Your money balance: [{(_appData.CurrentUser.Account.Sum == 0 ? "red" : "green")}]{_appData.CurrentUser.Account.Sum} $[/]");

            return BackToMenuPrompt("Top up money balance");
        }

        public string TopUpBalance() {
            ClearConsole();
            ShowHeader("Top up money balance");

            double amount = DefaultTextPrompt<double>("How much you want to [green]top up your money balance?[/] ");

            StatusSpinner("Sending...");

            try {
                _appData.CurrentUser.Value.TopUpAccount(amount);
            }
            catch (ArgumentException ex) {
                DefaultErrorOutput(ex.Message);
            }

            return CheckAccount();
        }
    }
}
