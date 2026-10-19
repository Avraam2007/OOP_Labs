using OOP_Main.Builders;
using OOP_Main.Comparers;
using OOP_Main.Entities;
using OOP_Main.Handlers;
using OOP_Main.UI;
using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main.Screens {
    public class OrderScreen: BaseScreen, IScreen {
        public OrderScreen(Data appData): base(appData) { }

        public string Show() {
            if (_appData.CurrentUser.IsEmpty()) {
                DefaultErrorOutput($"The access is forbidden");
                return BackToMenuPrompt(
                    GetEnumDescription(MenuOption.SignUp)
                );
            }

            ShowHeader("Orders");

            var sortOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose sorting option:")
                    .AddChoices("By ID (default)", "By total price (from the cheapest)")
            );

            var orders = CheckIfUserIsAdmin(_appData.CurrentUser) ?
                _appData.Orders :
                _appData.GetOrdersFromUser(_appData.CurrentUser.Value.Id);

            List<Order> ordersToSort = new List<Order>(orders);

            switch (sortOption) {
                case "By ID (default)":
                    ordersToSort.Sort(new OrderIDComparer());
                    break;
                case "By total price (from the cheapest)":
                    ordersToSort.Sort(new OrderPriceComparer());
                    break;
            }
            return ShowListScreen("Orders", ordersToSort, extraErrorMessage: "You can create it.");
        }

        public string Create() {
            ShowHeader("Creating order");

            if (Tools.ValidateArray(_appData.Products)) {
                DefaultErrorOutput("No products found. Call the admin to fix this issue.");
                return BackToMenuPrompt();
            }

            const string exitOption = "Exit";

            ProductCatalogCollection catalog = new ProductCatalogCollection();

            var availableProducts = new List<Product>(_appData.Products);

            var productNames = _appData.Products.Select(p => $"{p.Name} - {p.Price}$").ToList();

            productNames.Add(exitOption);

            bool isChosenExit = false;

            while (!isChosenExit) {
                List<string> categoriesList;

                try {
                    categoriesList = availableProducts.GetCategoriesList().ToList();
                }
                catch (NullReferenceException ex) {
                    DefaultErrorOutput(ex.Message);
                    return BackToMenuPrompt();
                }

                var selectedCategory = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose [green]product category[/]:")
                        .AddChoices(categoriesList)
                        .UseConverter(c => c == null ? exitOption : c.AddSpacesBetweenCapitalizedWords())
                        .AddChoices((string)null)
                );

                if (selectedCategory == null) {
                    isChosenExit = true;
                    continue;
                }

                var filteredProducts = availableProducts.FilterByCategory(selectedCategory);

                var prompt = new SelectionPrompt<Product>()
                    .Title("Select [green]product[/] to order")
                    .PageSize(15)
                    .UseConverter(p => p == null ? exitOption : $"{p.Name} - {p.Price}$")
                    .AddChoices(filteredProducts);

                var productChoice = AnsiConsole.Prompt(prompt);

                if (productChoice == null) {
                    DefaultErrorOutput($"Sorry, we didn't found this product. Try again");
                    isChosenExit = true;
                    continue;
                }

                int amount = DefaultTextPrompt<int>("Enter [green]amount[/] of this product");
                    
                if (amount <= 0) {
                    DefaultErrorOutput($"Amount should be more than 0. Try again");
                    continue;
                }

                for (int i = 0; i < amount; i++) {
                    catalog.Add(productChoice);
                }

                availableProducts.Remove(productChoice);

                ShowHeader("Creating order");
            }

            if (catalog.Count == 0) return BackToMenuPrompt();

            int userId = _appData.CurrentUser.Value.Id;
            int newOrderID = _appData.GetOrdersFromUser(userId).LastOrDefault().OrderId;

            var director = new Director();
            var builder = new OrderBuilder();
            director.Builder = builder;
            director.BuildValidationOrder(catalog.ToList());
            Order tempOrder = builder.GetOrder();

            var validationPipeline = new EmptyOrderValidationHandler();
            validationPipeline
                .SetNext(new UserAccountStatusHandler())
                .SetNext(new StockAvailabilityHandler())
                .SetNext(new InsufficientFundsHandler())
                .SetNext(new OrderAmountLimitHandler(maxOrderLimit: 5000.0));

            if (!validationPipeline.Handle(tempOrder, _appData.CurrentUser.Value, out string validationError)) {
                DefaultErrorOutput(validationError);
                return BackToMenuPrompt();
            }

            double totalPrice = tempOrder.GetTotalPrice();

            bool acceptOrder = DefaultConfirm($"The order costs {totalPrice}$. Would you like to pay for it?");

            if (!acceptOrder) return BackToMenuPrompt();

            try {
                _appData.CurrentUser.Value.TakeMoney(totalPrice);

                _appData.AddOrder(userId, catalog.ToList());
                DefaultSuccessOutput($"New transaction: -{totalPrice} $");

                Order newOrder = _appData.GetOrderById(newOrderID).GetValueOrElseThrow();

                newOrder.Pay();


                _appData.OrderQueue?.EnqueueOrder(newOrder);

                string orderReciept = RecieptGenerator.BuildOrderReceipt(newOrder, _appData.CurrentUser.Value);
                DefaultMarkupOutput($"[bold green rapidblink]YOUR NEW ORDER[/]");
                var panel = new Panel(orderReciept)
                    .BorderColor(Color.Blue)
                    .DoubleBorder()
                    .Padding(4, 2);

                AnsiConsole.Write(panel);

                string username = _appData.CurrentUser.MapValue(user => user.Username).GetValueOrElse(newOrder.BuyerId.ToString());
                string savePath = $"DataStorage/reciept_order_{newOrderID}_from_{username}.txt";

                RecieptGenerator.SaveReciept(orderReciept, savePath);
                DefaultMarkupOutput($"[blue]Reciept was saved in file:[/] [yellow]{savePath}[/]");
            }
            catch (InsufficientFundsException ex) {
                DefaultErrorOutput(ex.Message);
                _appData.DeleteOrder(newOrderID);
            }
            catch (ArgumentException ex) {
                DefaultErrorOutput(ex.Message);
                _appData.DeleteOrder(newOrderID);
            }
            catch (ECommerceException ex) {
                DefaultErrorOutput(ex.Message);
                _appData.DeleteOrder(newOrderID);
            }

            return BackToMenuPrompt();
        }

        public string Delete() => DeleteObjectScreen<Order, int>(
            _appData.Orders,
            _appData.DeleteOrder,
            order => $"[red]\"({order.OrderId}) from {order.BuyerId}\"[/]"
        );

        public string Edit() {
            ShowHeader("Edit order");

            var orderChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Order>()
                    .Title("Select order to edit:")
                    .UseConverter(o => o.ToString())
                    .AddChoices(_appData.Orders));

            string[] fieldNames = orderChoice.GetType().GetProperties().Select(f => f.Name).ToArray();
            var fieldChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select field to edit:")
                    .AddChoices(fieldNames));

            return BackToMenuPrompt();
        }
    }
}
