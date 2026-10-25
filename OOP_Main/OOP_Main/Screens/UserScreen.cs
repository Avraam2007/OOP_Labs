using NOptional;
using OOP_Main.Comparers;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.UI;
using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OOP_Main.Screens {
    public class UserScreen: BaseScreen, IShowScreen, IDeleteScreen, IEditScreen {
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

            return BackToMenuPrompt("Top up money balance", "Change password");
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

        public string ChangePassword() {
            ShowHeader("Change password");

            string oldPassword = PasswordPrompt("What's your [green]password[/]?");

            IOptional<User> currentUser = _appData.CurrentUser;

            if (currentUser.IsEmpty() || !UserService.CheckPassword(oldPassword, currentUser.Value) || currentUser.Value.IsGuest()) {
                DefaultErrorOutput($"Invalid password. Try again");
                return BackToMenuPrompt();
            }

            string newPassword = PasswordPrompt("Create your [green]new password[/]:");

            string confirmPassword = PasswordPrompt("Confirm your [green]new password[/]:");

            if (confirmPassword == newPassword) {
                try {
                    _appData.CurrentUser.Value.UpdatePassword(UserService.HashPassword(newPassword));
                    Task.Run(() => _appData.SaveAllData()).GetAwaiter().GetResult();

                    DefaultSuccessOutput("Password was changed!");
                }
                catch (ArgumentException e) {
                    DefaultErrorOutput(e.Message);
                    return BackToMenuPrompt();
                }
                return BackToMenuPrompt();
            }
            else {
                DefaultErrorOutput($"Incorrect password. Try again");
                return BackToMenuPrompt();
            }
        }

        public string Edit() => this.EditObjectScreen(_appData.Users);
    }
}
