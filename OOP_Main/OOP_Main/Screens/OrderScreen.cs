using NOptional;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main {
    public class OrderScreen: BaseScreen, IScreen {
        public OrderScreen(Data appData): base(appData) { }

        public string Show() {
            if (_appData.CurrentUser.IsEmpty()) {
                DefaultErrorOutput($"The access is forbidden");
                return BackToMenuPrompt(
                    GetEnumDescription(MenuOption.SignUp)
                );
            }
            return ShowListScreen(
                "Orders",
                CheckIfUserIsAdmin(_appData.CurrentUser) ?
                _appData.Orders :
                _appData.GetOrdersFromUser(_appData.CurrentUser.Value.Id),
                "You can create it."
            );
        }

        public string Create() {
            ClearConsole();
            ShowHeader("Creating order");

            if (Tools.ValidateArray(_appData.Products)) {
                DefaultErrorOutput("No products found. Call the admin to fix this issue.");
                return BackToMenuPrompt();
            }

            const string exitOption = "Exit";

            List<Product> catalog = new List<Product>();

            List<string> productNames = new List<string> {
                exitOption
            };

            bool isChosenExit = false;

            foreach (var product in _appData.Products) {
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

                IOptional<Product> foundProduct = _appData.GetProductByName(productChoice);
                if (foundProduct.HasValue()) {
                    Product chosenProduct = foundProduct.Value;
                    int amount = DefaultTextPrompt<int>("Enter [green]amount[/] of this product");
                    
                    if (amount <= 0) {
                        DefaultErrorOutput($"Amount should be more than 0. Try again");
                        continue;
                    }

                    for (int i = 0; i < amount; i++) {
                        catalog.Add(chosenProduct);
                    }
                }
                else {
                    DefaultErrorOutput($"Sorry, we didn't found this product. Try again");
                }
                productNames.Remove(productChoice);
                ClearConsole();
                ShowHeader("Creating order");
            }

            if (catalog.Count > 0) {
                int userId = _appData.CurrentUser.Value.Id;
                _appData.AddOrder(userId, catalog);
                int newOrderID = _appData.GetOrdersFromUser(userId).LastOrDefault().OrderId;

                Order newOrder = _appData.GetOrderById(newOrderID).Value;

                double totalPrice = newOrder.GetTotalPrice();

                bool acceptOrder = DefaultConfirm($"The order costs {totalPrice}$. Would you like to pay for it?");

                if (!acceptOrder) {
                    _appData.DeleteOrder(newOrderID);
                    return BackToMenuPrompt();
                }

                try {
                    _appData.CurrentUser.Value.TakeMoney(totalPrice);
                    DefaultSuccessOutput($"New transaction: -{totalPrice} $");
                    newOrder.Status = OrderStatus.Paid;
                    DefaultMarkupOutput($"[bold green rapidblink]YOUR NEW ORDER[/]");
                    InternalFlashCard(CardRenderer.GetCardInfo(newOrder));
                }
                catch (InsufficientFundsException ex) {
                    DefaultErrorOutput(ex.Message);
                    _appData.DeleteOrder(newOrderID);
                }
                catch (ArgumentException ex) {
                    DefaultErrorOutput(ex.Message);
                    _appData.DeleteOrder(newOrderID);
                }

                return BackToMenuPrompt();
            }

            return BackToMenuPrompt();
        }


        public string Delete() {
            ClearConsole();
            ShowHeader("Delete order");

            if (Tools.ValidateArray(_appData.Orders)) {
                DefaultErrorOutput("There are no orders to delete.");
                return BackToMenuPrompt();
            }

            var orderChoicesMap = new Dictionary<string, Order>();
            foreach (var order in _appData.Orders) {
                orderChoicesMap.Add(order.ToString(), order);
            }

            var choices = orderChoicesMap.Keys.ToList();

            choices.Add("Cancel");

            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a order to delete:")
                    .PageSize(10)
                    .AddChoices(choices));

            if (selectedChoice == "Cancel") return BackToMenuPrompt();
            IOptional<Order> orderToDelete = Optional.OfNullable(orderChoicesMap[selectedChoice]);

            if (orderToDelete.HasValue()) {
                int orderId = orderToDelete.Value.OrderId;
                bool confirm = DefaultConfirm(
                    $"Are you sure you want to delete this order ({orderId})?",
                    defaultValue: false
                );

                if (confirm) {
                    bool isDeleted = _appData.DeleteOrder(orderId);
                    if (isDeleted) {
                        DefaultSuccessOutput("Order successfully deleted!");
                    }
                    else {
                        DefaultErrorOutput("Failed to delete order.");
                    }
                }
                else {
                    DefaultMarkupOutput("[yellow]Deleting cancelled.[/]");
                }
            }

            return BackToMenuPrompt();
        }
    }
}
