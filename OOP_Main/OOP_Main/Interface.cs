using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OOP_Main {
    public class Interface {
        private Data appData;

        public Data AppData { get => appData; set => appData = value; }

        private enum MenuOption {
            [Description("Show users")] ShowUsers,
            [Description("Show orders")] ShowOrders,
            [Description("Show products")] ShowProducts,
            [Description("Create order")] CreateOrder,
            [Description("Add product")] AddProduct,
            [Description("Sign up")] SignUp,
            [Description("Log in")] LogIn,
            [Description("Quit")] Quit
        }

        private string GetEnumDescription(Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public Interface(Data appData) {
            this.AppData = appData;
        }
        private void BackToMenu(string extraOption = "") {
            List<string> options = new List<string> { "Back to menu" };
            if (extraOption != "") {
                options.Add(extraOption);
            }
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"\nPress ESC or Enter to back to menu{(extraOption != "" ? $" or choose {extraOption} option" : "")}")
                    .AddCancelResult("Back to menu")
                    .DefaultValue("Back to menu")
                    .AddChoices(options));
            switch (choice) {
                case "Back to menu":
                    MainScreen();
                    break;
                case "Sign up":
                    CreateUserScreen();
                    break;
                case "Log in":
                    LoginScreen();
                    break;
                default:
                    break;
            }
        }

        private void ShowHeader(string text) {
            var logoPanel = new Panel(
                new Markup(text, new Style(foreground: Color.Blue))
                )
                .BorderColor(Color.Blue)
                .Padding(2, 2);
            var header = new Padder(logoPanel);

            AnsiConsole.Write(header);
            AnsiConsole.Write(new Rule());
        }

        private void InternalFlashCard(Dictionary<string, Spectre.Console.Text> dataForCard) {
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

        private void ShowListScreen<T>(string header, IEnumerable<T> items, string extraErrorMessage = "") where T : class {
            AnsiConsole.Clear();
            ShowHeader(header);
            if (items == null || !items.Any()) {
                AnsiConsole.Markup($"[red bold]Sorry, we didn't find any {header.ToLower()} in the store. {extraErrorMessage}[/]");
            }
            else {
                foreach (var item in items) {
                    var dataForCard = ((dynamic)item).ShowInfo();
                    InternalFlashCard(dataForCard);
                }
            }
            BackToMenu();
        }

        public void ShowOrdersScreen() => ShowListScreen("Orders", appData.Orders, "You can create it.");

        public void ShowProductsScreen() => ShowListScreen("Products", appData.Products);

        public void ShowUsersScreen() => ShowListScreen("Users", appData.Users);

        public void StatusSpinner(string text, int loadingTimeInMs = 2400) {
            AnsiConsole.Status()
                .Start(text, ctx => {
                    Thread.Sleep(loadingTimeInMs);
                });
        }
        public void BootUpScreen() {
            // Styled text with markup
            AnsiConsole.MarkupLine("[bold blue]ECommerce[/] [green]v0.9[/]");

            // Status spinner for work
            StatusSpinner("Loading...");
            MainScreen();
        }

        public void BootDownScreen() {
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
                    Environment.Exit(0);
                    break;
                case "No":
                    MainScreen();
                    break;
                default:
                    break;
            }
            
        }

        public void CreateUserScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Sign up[/]");
            var usernamePrompt = new TextPrompt<string>("Enter your [green]username[/]:");

            string username = AnsiConsole.Prompt(usernamePrompt);

            var passwordPrompt = new TextPrompt<string>("Enter your [green]password[/]:")
                .Secret();

            string password = AnsiConsole.Prompt(passwordPrompt);

            var confirmPasswordPrompt = new TextPrompt<string>("Confirm your [green]password[/]:")
                .Secret();

            string confirmPassword = AnsiConsole.Prompt(confirmPasswordPrompt);

            if (confirmPassword == password) {
                AppData.CurrentUser = new User("258", username, password);
                AppData.AddUser(AppData.CurrentUser);
                AnsiConsole.MarkupLine($"[green bold]Account was created![/]");
                BackToMenu();
            }
        }

        public void AddProductScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Adding product[/]");
            var prompt = new SelectionPrompt<string>()
                .Title("Select product [green]category[/]")
                .PageSize(15)
                .AddChoiceGroup("Electronic product", new[]
                {
                    "Electronic product"
                })
                .AddChoiceGroup("Cloth", new[]
                {
                    "Cloth"
                })
                .AddChoiceGroup("Furniture", new[]
                {
                    "Sofa", "Other furniture"
                });
            var categoryChoice = AnsiConsole.Prompt(prompt);

            AnsiConsole.Clear();
            ShowHeader("[bold]Adding product[/]");
            var articlePrompt = new TextPrompt<string>("Enter product [green]article[/]:");

            string articleChoice = AnsiConsole.Prompt(articlePrompt);

            var namePrompt = new TextPrompt<string>("Enter product [green]name[/]:");

            string nameChoice = AnsiConsole.Prompt(namePrompt);

            var pricePrompt = new TextPrompt<double>("Enter product [green]price (in dollars)[/]:");

            double priceChoice = AnsiConsole.Prompt(pricePrompt);

            List<string> suppliers = new List<string>();
            foreach (var supplier in AppData.Suppliers) {
                suppliers.Add(supplier.Name);
            }
            var supplierPrompt = new SelectionPrompt<string>()
                .Title("Select product [green]supplier[/]")
                .PageSize(15)
                .AddChoices(suppliers);

            string supplierChoice = AnsiConsole.Prompt(supplierPrompt);

            switch (categoryChoice) {
                case "Electronic product":
                    var powerPrompt = new TextPrompt<int>("Enter product [green]power (in watts)[/]:");

                    int powerChoice = AnsiConsole.Prompt(powerPrompt);

                    var maxVoltagePrompt = new TextPrompt<int>("Enter product [green]max voltage (in volts)[/]:");

                    int maxVoltageChoice = AnsiConsole.Prompt(maxVoltagePrompt);

                    ElectronicProduct newProduct = new ElectronicProduct(
                        articleChoice, 
                        nameChoice, 
                        priceChoice, 
                        powerChoice, 
                        maxVoltageChoice, 
                        appData.GetSupplierByName(supplierChoice).SupplierId
                    );
                    appData.AddProduct(newProduct);
                    appData.GetSupplierByName(supplierChoice).AddPartToCatalog(newProduct);
                    AnsiConsole.MarkupLine($"[green bold]New product is created! You can check it on \"{GetEnumDescription(MenuOption.ShowProducts)}\" screen.[/]");
                    break;
                case "Cloth":
                    break;
                case "Sofa":
                    break;
                case "Other furniture":
                    break;
                default:
                    AnsiConsole.MarkupLine($"[red bold]Unknown error[/]");
                    break;
            }

            BackToMenu();
        }

        public void LoginScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Log in[/]");
            var usernamePrompt = new TextPrompt<string>("What's your [green]username[/]?");

            string username = AnsiConsole.Prompt(usernamePrompt);

            if (AppData.GetUserByUsername(username) == null) {
                AnsiConsole.MarkupLine($"[red bold]Sorry, we didn't found this account. Try again or sign in this account[/]");
                BackToMenu(
                    GetEnumDescription(MenuOption.SignUp)
                );
            }

            var passwordPrompt = new TextPrompt<string>("What's your [green]password[/]?")
                .Secret();

            string password = AnsiConsole.Prompt(passwordPrompt);

            if (AppData.GetUserByUsername(username).Password != password ) {
                AnsiConsole.MarkupLine($"[red bold]Invalid username or password. Try again[/]");
                BackToMenu();
            }
            else {
                if (AppData.CurrentUser == null || AppData.CurrentUser != AppData.GetUserByUsername(username)) {
                    AppData.CurrentUser = AppData.GetUserByUsername(username);
                }
                AnsiConsole.MarkupLine($"[green bold]Welcome back, {username}![/]");
            }
        }

        private void CheckAccessToCreateOrder() {
            if (AppData.CurrentUser == null) {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden[/]");
                BackToMenu(
                    GetEnumDescription(MenuOption.SignUp)
                );
            }
            else {
                AnsiConsole.MarkupLine($"[red bold]You should log in first[/]");
                BackToMenu(
                    GetEnumDescription(MenuOption.LogIn)
                );
            }
        }

        private void CheckIfUserIsAdmin(Action GoToPage) {
            if (AppData.CurrentUser != null && AppData.CurrentUser.IsAdmin) {
                GoToPage();
            }
            else {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden. Only for admin[/]");
                BackToMenu();
            }
        }

        private string GetOptionName(MenuOption option) {
            switch (option) {
                case MenuOption.ShowUsers: return "Show users";
                case MenuOption.ShowOrders: return "Show orders";
                case MenuOption.ShowProducts: return "Show products";
                case MenuOption.CreateOrder: return "Create order";
                case MenuOption.AddProduct: return "Add product";
                case MenuOption.SignUp: return "Sign up";
                case MenuOption.LogIn: return "Log in";
                case MenuOption.Quit: return "Quit";
                default: return option.ToString();
            }
        }

        public void MainScreen() {
            AnsiConsole.Clear();

            ShowHeader("[bold]Welcome to [green]ECommerce[/][/]");

            List<MenuOption> menuList = new List<MenuOption>{
                        MenuOption.ShowUsers,
                        MenuOption.ShowOrders,
                        MenuOption.ShowProducts,
                        MenuOption.CreateOrder,
                        MenuOption.AddProduct,
                        MenuOption.SignUp,
                        MenuOption.LogIn,
                        MenuOption.Quit
            };

            var prompt = new SelectionPrompt<MenuOption>()
                    .Title("Choose option")
                    .WrapAround()
                    .AddChoices(menuList);

            prompt.UseConverter(new Func<MenuOption, string>(GetOptionName));

            var choice = AnsiConsole.Prompt(prompt);

            switch (choice) {
                case MenuOption.ShowUsers:
                    CheckIfUserIsAdmin(ShowUsersScreen);
                    break;
                case MenuOption.ShowOrders:
                    ShowOrdersScreen();
                    break;
                case MenuOption.ShowProducts:
                    ShowProductsScreen();
                    break;
                case MenuOption.CreateOrder:
                    CheckAccessToCreateOrder();
                    break;
                case MenuOption.AddProduct:
                    CheckIfUserIsAdmin(AddProductScreen);
                    break;
                case MenuOption.SignUp:
                    CreateUserScreen();
                    break;
                case MenuOption.LogIn:
                    LoginScreen();
                    BackToMenu();
                    break;
                case MenuOption.Quit:
                    BootDownScreen();
                    break;
                default:
                    break;
            }
        }
    }
}
