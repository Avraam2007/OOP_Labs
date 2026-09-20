using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class UserScreen: UIHelper {
        private readonly Data _appData;
        public UserScreen(Data appData) {
            _appData = appData;
        }

        public string ShowUsersScreen() => ShowListScreen("Users", _appData.Users);

        public string DeleteUserScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Delete user[/]");

            var regularUsers = new List<User>();

            _appData.Users.ForEach(user => { if (!user.IsAdmin) regularUsers.Add(user); });


            if (Tools.ValidateArray(regularUsers)) {
                AnsiConsole.MarkupLine("[red bold]There are no users to delete.[/]");
                return BackToMenuPrompt();
            }

            var choices = new List<string>();
            foreach (var user in regularUsers) {
                choices.Add($"({user.Id}) {user.Username}");
            }
            choices.Add("Cancel");

            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an user to delete:")
                    .PageSize(10)
                    .AddChoices(choices));

            if (selectedChoice == "Cancel") return BackToMenuPrompt();

            string userId = selectedChoice.Split(')')[0].TrimStart('(');
            User userToDelete = _appData.Users.FirstOrDefault(u => u.Id == userId);

            if (userToDelete != null) {
                bool confirm = AnsiConsole.Confirm(
                    $"Are you sure you want to delete [red]\"{userToDelete.Username}\"[/] ({userToDelete.Id})?",
                    defaultValue: false
                );

                if (confirm) {
                    bool isDeleted = _appData.DeleteUserById(userToDelete.Id);
                    if (isDeleted) {
                        AnsiConsole.MarkupLine("[green]User successfully deleted![/]");
                    }
                    else {
                        AnsiConsole.MarkupLine("[red]Failed to delete user.[/]");
                    }
                }
                else {
                    AnsiConsole.MarkupLine("[yellow]Deleting cancelled.[/]");
                }
            }

            return BackToMenuPrompt();
        }
    }
}
