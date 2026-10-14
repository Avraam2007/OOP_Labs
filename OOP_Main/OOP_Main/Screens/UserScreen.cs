using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OOP_Main.Screens {
    public class UserScreen: BaseScreen, IShowScreen, IDeleteScreen {
        public UserScreen(Data appData): base(appData) { }

        public string Show() => ShowListScreen("Users", _appData.Users);

        public string Delete() => DeleteObjectScreen<User, int>(
            _appData.Users, 
            _appData.DeleteUserById,
            user => $"[red]\"{user.Username}\"[/] ({user.Id})"
        );

        public string CheckAccount() {
            ClearConsole();
            ShowHeader("My account");

            InternalFlashCard(CardRenderer.GetCardInfo(_appData.CurrentUser.Value));

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
