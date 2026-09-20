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
            string nextAction = _authScreen.CreateUserScreen();
            HandleNavigation(nextAction);
        }

        public void BootUpScreen() {
            // Styled text with markup
            AnsiConsole.MarkupLine("[bold blue]ECommerce[/] [green]v0.12[/]");

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
            return _orderScreen.CreateOrderScreen();
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

        private string GetOptionName(MenuOption option) {
            switch (option) {
                case MenuOption.ShowUsers: return "Show users";
                case MenuOption.ShowOrders: return "Show orders";
                case MenuOption.ShowProducts: return "Show products";
                case MenuOption.ShowSuppliers: return "Show suppliers";
                case MenuOption.CreateOrder: return "Create order";
                case MenuOption.AddProduct: return "Add product";
                case MenuOption.AddSupplier: return "Add supplier";
                case MenuOption.SignUp: return "Sign up";
                case MenuOption.LogIn: return "Log in";
                case MenuOption.Quit: return "Quit";
                default: return option.ToString();
            }
        }

        public bool MainScreen() {
            AnsiConsole.Clear();

            ShowHeader("[bold]Welcome to [green]ECommerce[/][/]");

            var menuList = new List<MenuOption>{
                        MenuOption.ShowUsers,
                        MenuOption.ShowOrders,
                        MenuOption.ShowProducts,
                        MenuOption.ShowSuppliers,
                        MenuOption.CreateOrder,
                        MenuOption.AddProduct,
                        MenuOption.AddSupplier,
                        MenuOption.SignUp,
                        MenuOption.LogIn,
                        MenuOption.Quit
            };

            var prompt = new SelectionPrompt<MenuOption>()
                    .Title("Choose option")
                    .WrapAround()
                    .PageSize(10)
                    .AddChoices(menuList);

            prompt.UseConverter(new Func<MenuOption, string>(GetOptionName));

            var choice = AnsiConsole.Prompt(prompt);

            switch (choice) {
                case MenuOption.ShowUsers:
                    CheckAdminAccessToPage(_userScreen.ShowUsersScreen);
                    break;
                case MenuOption.ShowOrders:
                    _orderScreen.ShowOrdersScreen();
                    break;
                case MenuOption.ShowProducts:
                    _productScreen.ShowProductsScreen();
                    break;
                case MenuOption.ShowSuppliers:
                    _supplierScreen.ShowSuppliersScreen();
                    break;
                case MenuOption.CreateOrder:
                    CheckAccessToCreateOrder();
                    break;
                case MenuOption.AddProduct:
                    CheckAdminAccessToPage(_productScreen.AddProductScreen);
                    break;
                case MenuOption.AddSupplier:
                    CheckAdminAccessToPage(_supplierScreen.AddSupplierScreen);
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
