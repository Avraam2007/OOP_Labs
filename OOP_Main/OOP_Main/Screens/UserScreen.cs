using OOP_Main.Comparers;
using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OOP_Main.Screens {
    public class UserScreen: BaseScreen, IShowScreen, IDeleteScreen {
        public UserScreen(Data appData): base(appData) { }

        public string Show() {
            ShowHeader("Users");

            var sortOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose sorting option:")
                    .AddChoices("By ID (default)", "By status (A-Z)", "By name (A-Z)")
            );

            List<User> usersToSort = new List<User>(_appData.Users);

            switch (sortOption) {
                case "By ID (default)":
                    usersToSort.Sort(new UserIDComparer());
                    break;
                case "By name (A-Z)":
                    usersToSort.Sort(new UsernameComparer());
                    break;
                case "By status (A-Z)":
                    usersToSort.Sort(new UserRoleComparer());
                    break;
            }
            return ShowListScreen("Users", usersToSort);
        }

        public string Delete() => DeleteObjectScreen<User, int>(
            _appData.Users, 
            _appData.DeleteUserById,
            user => $"[red]\"{user.Username}\"[/] ({user.Id})"
        );

        public string CheckAccount() {
            ShowHeader("My account");

            InternalFlashCard(CardRenderer.GetCardInfo(_appData.CurrentUser.Value));

            return BackToMenuPrompt("Top up money balance");
        }

        public string TopUpBalance() {
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
