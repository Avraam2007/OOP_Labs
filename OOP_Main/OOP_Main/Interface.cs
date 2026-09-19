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
        public Data AppData { get; set; }

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

        public Interface(Data AppData) {
            this.AppData = AppData;
        }

        private string GetEnumDescription(Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null) return value.ToString();
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        private void BackToMenu(string extraOption = "") {
            List<string> options = new List<string> { "Back to menu" };
            if (!string.IsNullOrWhiteSpace(extraOption)) {
                options.Add(extraOption);
            }
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"\nPress ESC or Enter to back to menu{(extraOption != "" ? $" or choose {extraOption} option" : "")}")
                    .AddCancelResult("Back to menu")
                    .DefaultValue("Back to menu")
                    .AddChoices(options));
            switch (choice) {
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

        private void ShowListScreen<T>(string header, IEnumerable<T> items, string extraErrorMessage = "") where T : class, IDataAndUIBridge {
            AnsiConsole.Clear();
            ShowHeader(header);

            if (items == null || !items.Any()) {
                AnsiConsole.Markup($"[red bold]Sorry, we didn't find any {header.ToLower()} in the store. {extraErrorMessage}[/]");
            }

            foreach (var item in items) {
                var dataForCard = item.ShowInfo();
                InternalFlashCard(dataForCard);
            }

            BackToMenu();
        }


        public void ShowOrdersScreen() {
            if (AppData.CurrentUser == null) {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden[/]");
                BackToMenu(
                    GetEnumDescription(MenuOption.SignUp)
                );
                return;
            }
            ShowListScreen(
                "Orders",
                CheckIfUserIsAdmin() ?
                AppData.Orders :
                AppData.GetOrdersFromUser(AppData.CurrentUser.Id),
                "You can create it."
            );
        }

        public void ShowProductsScreen() => ShowListScreen("Products", AppData.Products);

        public void ShowUsersScreen() => ShowListScreen("Users", AppData.Users);

        public void StatusSpinner(string text, int loadingTimeInMs = 2400) {
            AnsiConsole.Status()
                .Start(text, ctx => {
                    Thread.Sleep(loadingTimeInMs);
                });
        }
        public void BootUpScreen() {
            // Styled text with markup
            AnsiConsole.MarkupLine("[bold blue]ECommerce[/] [green]v0.11[/]");

            // Status spinner for work
            StatusSpinner("Loading...");
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
                    Environment.Exit(0);
                    return false;
                default:
                    return true;
            }

        }

        public void CreateUserScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Sign up[/]");
            var usernamePrompt = new TextPrompt<string>("Enter your [green]username[/]:");

            string username = AnsiConsole.Prompt(usernamePrompt);

            if (AppData.GetUserByUsername(username) != null) {
                AnsiConsole.MarkupLine($"[red bold]Sorry, this account was created earlier. You can log in to this account instead[/]");
                BackToMenu(
                    GetEnumDescription(MenuOption.LogIn)
                );
            }

            var passwordPrompt = new TextPrompt<string>("Create your [green]password[/]:")
                .Secret();

            string password = AnsiConsole.Prompt(passwordPrompt);

            var confirmPasswordPrompt = new TextPrompt<string>("Confirm your [green]password[/]:")
                .Secret();

            string confirmPassword = AnsiConsole.Prompt(confirmPasswordPrompt);

            if (confirmPassword == password) {
                string newId = new Random().Next(100, 999).ToString();
                AppData.CurrentUser = new User(newId, username, password);
                AppData.AddUser(AppData.CurrentUser);
                AnsiConsole.MarkupLine($"[green bold]Account was created![/]");
                BackToMenu();
            }
        }

        public void AddingProduct(Product newProduct, string supplierChoice) {
            AppData.AddProduct(newProduct);
            AppData.GetSupplierByName(supplierChoice).AddProductToCatalog(newProduct);
            AnsiConsole.MarkupLine($"[green bold]New product is created! You can check it on \"{GetEnumDescription(MenuOption.ShowProducts)}\" screen.[/]");
        }

        public void AddProductScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Adding product[/]");

            if (Tools.ValidateArray(AppData.Suppliers)) {
                AnsiConsole.MarkupLine("[red bold]No suppliers found. Call the admin to fix this issue[/]");
                BackToMenu();
                return;
            }

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

            int supplierId = AppData.GetSupplierByName(supplierChoice).SupplierId;

            switch (categoryChoice) {
                case "Electronic product":
                    int powerChoice = AnsiConsole.Prompt(new TextPrompt<int>("Enter product [green]power (in watts)[/]:"));
                    int maxVoltageChoice = AnsiConsole.Prompt(new TextPrompt<int>("Enter product [green]max voltage (in volts)[/]:"));

                    var newElectronicProduct = new ElectronicProduct(
                        articleChoice,
                        nameChoice,
                        priceChoice,
                        powerChoice,
                        maxVoltageChoice,
                        supplierId
                    );
                    AddingProduct(newElectronicProduct, supplierChoice);
                    break;

                case "Cloth":
                    string materialChoice = AnsiConsole.Prompt(new TextPrompt<string>("Enter product [green]material[/]:"));

                    var newCloth = new Cloth(
                        articleChoice,
                        nameChoice,
                        priceChoice,
                        materialChoice,
                        supplierId
                    );
                    AddingProduct(newCloth, supplierChoice);
                    break;

                case "Sofa":
                case "Other furniture":
                    double weight = AnsiConsole.Prompt(new TextPrompt<double>("Enter product [green]weight (in kg)[/]:"));
                    double width = AnsiConsole.Prompt(new TextPrompt<double>("Enter product [green]width (in cm)[/]:"));
                    double length = AnsiConsole.Prompt(new TextPrompt<double>("Enter product [green]length (in cm)[/]:"));
                    double height = AnsiConsole.Prompt(new TextPrompt<double>("Enter product [green]height (in cm)[/]:"));
                    string mat = AnsiConsole.Prompt(new TextPrompt<string>("Enter product [green]material[/]:"));

                    if (categoryChoice == "Sofa") {
                        bool assemble = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("Is the product [green]assembled[/]?")
                                .AddChoices(true, false));

                        var newSofa = new Sofa(
                            articleChoice,
                            nameChoice,
                            priceChoice,
                            weight,
                            width,
                            length,
                            height,
                            mat,
                            assemble,
                            supplierId
                        );
                        AddingProduct(newSofa, supplierChoice);
                    }
                    else {
                        var newFurniture = new Furniture(
                            articleChoice,
                            nameChoice,
                            priceChoice,
                            weight,
                            width,
                            length,
                            height,
                            mat,
                            supplierId
                        );
                        AddingProduct(newFurniture, supplierChoice);
                    }
                    break;
            }

            BackToMenu();
        }

        public void CreateOrderScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Creating order[/]");
            
            if (Tools.ValidateArray(AppData.Products)) {
                AnsiConsole.MarkupLine("[red bold]No products found. Call the admin to fix this issue.[/]");
                BackToMenu();
                return;
            }
            
            const string exitOption = "Exit";
            Order currentOrder = new Order(new Random().Next(100, 999), AppData.CurrentUser.Id);

            List<string> productNames = new List<string> {
                exitOption
            };

            bool isChosenExit = false;

            foreach (var product in AppData.Products) {
                productNames.Add(product.Name);
            }

            while (!isChosenExit) {
                var prompt = new SelectionPrompt<string>()
                    .Title("Select [green]product[/] to order")
                    .PageSize(15)
                    .AddChoices(productNames);
                var productChoice = AnsiConsole.Prompt(prompt);

                if (productChoice == exitOption) {
                    isChosenExit = true;
                    continue;
                }

                Product chosenProduct = AppData.GetProductByName(productChoice);
                if (chosenProduct != null) {
                    int amount = AnsiConsole.Ask<int>("Enter [green]amount[/] of this product");

                    if (amount > 0) {
                        for (int i = 0; i < amount; i++) {
                            currentOrder.AddProduct(chosenProduct);
                        }
                    }
                    else {
                        AnsiConsole.MarkupLine($"[red bold]Amount should be more than 0. Try again[/]");
                        continue;
                    }
                }
                else {
                    AnsiConsole.MarkupLine($"[red bold]Sorry, we didn't found this product. Try again[/]");
                }
                productNames.Remove(productChoice);
                AnsiConsole.Clear();
                ShowHeader("[bold]Creating order[/]");
            }

            if (currentOrder.Products.Count > 0) {
                AppData.AddOrder(currentOrder);
                AnsiConsole.MarkupLine($"[bold green rapidblink]YOUR NEW ORDER[/]");
                InternalFlashCard(currentOrder.ShowInfo());
            }

            BackToMenu();
        }

        public void LoginScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Log in[/]");
            var usernamePrompt = new TextPrompt<string>("What's your [green]username[/]?");

            string username = AnsiConsole.Prompt(usernamePrompt);

            var passwordPrompt = new TextPrompt<string>("What's your [green]password[/]?")
                .Secret();

            string password = AnsiConsole.Prompt(passwordPrompt);

            User user = AppData.GetUserByUsername(username);

            if (user == null || user.Password != password) {
                AnsiConsole.MarkupLine($"[red bold]Invalid username or password. Try again[/]");
                BackToMenu();
                return;

            }
            else {
                if (AppData.CurrentUser == null || AppData.CurrentUser != user) {
                    AppData.CurrentUser = user;
                }
                AnsiConsole.MarkupLine($"[green bold]Welcome back, {username}![/]");
            }
            BackToMenu();
        }

        private void CheckAccessToCreateOrder() {
            if (AppData.CurrentUser == null) {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden[/]");
                BackToMenu(
                    GetEnumDescription(MenuOption.SignUp)
                );
                return;
            }
            CreateOrderScreen();
        }

        private bool CheckIfUserIsAdmin() {
            return AppData.CurrentUser != null && AppData.CurrentUser.IsAdmin;
        }

        private void CheckAdminAccessToPage(Action GoToPage) {
            if (CheckIfUserIsAdmin()) {
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

        public bool MainScreen() {
            AnsiConsole.Clear();

            ShowHeader("[bold]Welcome to [green]ECommerce[/][/]");

            var menuList = new List<MenuOption>{
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
                    CheckAdminAccessToPage(ShowUsersScreen);
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
                    CheckAdminAccessToPage(AddProductScreen);
                    break;
                case MenuOption.SignUp:
                    CreateUserScreen();
                    break;
                case MenuOption.LogIn:
                    LoginScreen();
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
