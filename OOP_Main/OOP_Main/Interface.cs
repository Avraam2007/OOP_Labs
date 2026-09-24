using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Interface: UIHelper {
        private readonly AuthScreen _authScreen;
        private readonly ProductScreen _productScreen;
        private readonly OrderScreen _orderScreen;
        private readonly UserScreen _userScreen;
        private readonly SupplierScreen _supplierScreen;
        public Data AppData { get; set; }

        public Interface(Data AppData) {
            this.AppData = AppData;
            _authScreen = new AuthScreen(AppData);
            _productScreen = new ProductScreen(AppData);
            _orderScreen = new OrderScreen(AppData);
            _userScreen = new UserScreen(AppData);
            _supplierScreen = new SupplierScreen(AppData);
        }

        private void HandleNavigation(string nextAction) {
            switch (nextAction) {
                case "Sign up":
                    RunSignUpFlow();
                    break;
                case "Log in":
                    RunLoginFlow();
                    break;
                default:
                    break;
            }
        }

        public void RunLoginFlow() {
            string nextAction = _authScreen.LoginScreen();
            HandleNavigation(nextAction);
        }

        public void RunSignUpFlow() {
            string nextAction = _authScreen.Create();
            HandleNavigation(nextAction);
        }

        public void BootUpScreen() {
            // Styled text with markup
            AnsiConsole.MarkupLine("[bold blue]ECommerce[/] [green]v0.14[/]");

            // Status spinner for work
            StatusSpinner("Loading...");
            AppData.LoadAllData();
        }

        public void Start() {
            BootUpScreen();
            bool isRunning = true;
            while (isRunning) {
                isRunning = MainScreen();
            }
        }

        public bool BootDownScreen() {
            AnsiConsole.Clear();
            var exitChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Are you sure you want to quit the app?")
                    .AddCancelResult("No")
                    .DefaultValue("No")
                    .AddChoices("Yes", "No"));

            switch (exitChoice) {
                case "Yes":
                    StatusSpinner("Shutting down...");
                    AppData.SaveAllData();
                    Environment.Exit(0);
                    return false;
                default:
                    return true;
            }

        }

        private string CheckAccessToCreateOrder() {
            if (AppData.CurrentUser == null) {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden[/]");
                return BackToMenuPrompt(
                    GetEnumDescription(MenuOption.SignUp)
                );
            }
            return _orderScreen.Create();
        }

        private string CheckAdminAccessToPage(Func<string> GoToPage) {
            if (CheckIfUserIsAdmin(AppData.CurrentUser)) {
                return GoToPage();
            }
            else {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden. Only for admin[/]");
                return BackToMenuPrompt();
            }
        }
        private MenuOption MainMenuList(List<MenuOption> menuList) {
            var prompt = new SelectionPrompt<MenuOption>()
                .Title("Choose option")
                .WrapAround()
                .PageSize(10)
                .AddChoices(menuList);

            prompt.UseConverter(option => GetEnumDescription(option));

            return AnsiConsole.Prompt(prompt);
        }

        public bool MainScreen() {
            AnsiConsole.Clear();

            ShowHeader("[bold]Welcome to [green]ECommerce[/][/]");

            List<MenuOption> guestMenuList = new List<MenuOption>{
                        MenuOption.ShowProducts,
                        MenuOption.ShowSuppliers,
                        MenuOption.SignUp,
                        MenuOption.LogIn,
                        MenuOption.Quit
            };

            var menuList = new List<MenuOption>{
                        MenuOption.ShowOrders,
                        MenuOption.ShowProducts,
                        MenuOption.ShowSuppliers,
                        MenuOption.CreateOrder,
                        MenuOption.SignUp,
                        MenuOption.LogIn,
                        MenuOption.Quit
            };

            var adminMenuList = new List<MenuOption>{
                        MenuOption.ShowUsers,
                        MenuOption.ShowOrders,
                        MenuOption.ShowProducts,
                        MenuOption.ShowSuppliers,
                        MenuOption.CreateOrder,
                        MenuOption.AddProduct,
                        MenuOption.AddSupplier,
                        MenuOption.ChangeProductPrice,
                        MenuOption.DeleteOrder,
                        MenuOption.DeleteProduct,
                        MenuOption.DeleteUser,
                        MenuOption.DeleteSupplier,
                        MenuOption.SignUp,
                        MenuOption.LogIn,
                        MenuOption.Quit
            };

            MenuOption choice;

            if (CheckIfUserIsAdmin(AppData.CurrentUser)) {
                choice = MainMenuList(adminMenuList);
            }
            else if (AppData.CurrentUser != null) {
                choice = MainMenuList(menuList);
            }
            else {
                choice = MainMenuList(guestMenuList);
            }

            switch (choice) {
                    case MenuOption.ShowUsers:
                        CheckAdminAccessToPage(_userScreen.Show);
                        break;
                    case MenuOption.ShowOrders:
                        _orderScreen.Show();
                        break;
                    case MenuOption.ShowProducts:
                        _productScreen.Show();
                        break;
                    case MenuOption.ShowSuppliers:
                        _supplierScreen.Show();
                        break;
                    case MenuOption.CreateOrder:
                        CheckAccessToCreateOrder();
                        break;
                    case MenuOption.AddProduct:
                        CheckAdminAccessToPage(_productScreen.Create);
                        break;
                    case MenuOption.AddSupplier:
                        CheckAdminAccessToPage(_supplierScreen.Create);
                        break;
                    case MenuOption.ChangeProductPrice:
                        CheckAdminAccessToPage(_productScreen.Edit);
                        break;
                    case MenuOption.DeleteProduct:
                        CheckAdminAccessToPage(_productScreen.Delete);
                        break;
                    case MenuOption.DeleteUser:
                        CheckAdminAccessToPage(_userScreen.Delete);
                        break;
                    case MenuOption.DeleteSupplier:
                        CheckAdminAccessToPage(_supplierScreen.Delete);
                        break;
                    case MenuOption.DeleteOrder:
                        CheckAdminAccessToPage(_orderScreen.Delete);
                        break;
                    case MenuOption.SignUp:
                        RunSignUpFlow();
                        break;
                    case MenuOption.LogIn:
                        RunLoginFlow();
                        break;
                    case MenuOption.Quit:
                        return BootDownScreen();
                    default:
                        break;
                }

            return true;
        }
    }
}
