using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class UserScreen: BaseScreen {
        public UserScreen(Data appData): base(appData) { }

        public override string Create() => throw new NotImplementedException();

        public override string Show() => ShowListScreen("Users", _appData.Users);

        public override string Delete() {
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

            int userId = Convert.ToInt32(selectedChoice.Split(')')[0].TrimStart('('));

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
