using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class OrderScreen: BaseScreen {
        public OrderScreen(Data appData): base(appData) { }

        public override string Show() {
            if (_appData.CurrentUser == null) {
                DefaultMarkupOutput($"[red bold]The access is forbidden[/]");
                return BackToMenuPrompt(
                    GetEnumDescription(MenuOption.SignUp)
                );
            }
            return ShowListScreen(
                "Orders",
                CheckIfUserIsAdmin(_appData.CurrentUser) ?
                _appData.Orders :
                _appData.GetOrdersFromUser(_appData.CurrentUser.Id),
                "You can create it."
            );
        }

        public override string Create() {
            ClearConsole();
            ShowHeader("[bold]Creating order[/]");

            if (Tools.ValidateArray(_appData.Products)) {
                DefaultMarkupOutput("[red bold]No products found. Call the admin to fix this issue.[/]");
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

                Product chosenProduct = _appData.GetProductByName(productChoice);
                if (chosenProduct != null) {
                    int amount = DefaultTextPrompt<int>("Enter [green]amount[/] of this product");

                    if (amount > 0) {
                        for (int i = 0; i < amount; i++) {
                            catalog.Add(chosenProduct);
                        }
                    }
                    else {
                        DefaultMarkupOutput($"[red bold]Amount should be more than 0. Try again[/]");
                        continue;
                    }
                }
                else {
                    DefaultMarkupOutput($"[red bold]Sorry, we didn't found this product. Try again[/]");
                }
                productNames.Remove(productChoice);
                ClearConsole();
                ShowHeader("[bold]Creating order[/]");
            }

            if (catalog.Count > 0) {
                _appData.CurrentUser.RegisterAccountHandler(DefaultMarkupOutput);

                int userId = _appData.CurrentUser.Id;
                _appData.AddOrder(userId, catalog);
                int newOrderID = _appData.GetOrdersFromUser(userId).LastOrDefault().OrderId;

                Order newOrder = _appData.GetOrderById(newOrderID);

                double totalPrice = newOrder.GetTotalPrice();

                bool acceptOrder = DefaultConfirm($"The order costs {totalPrice}$. Would you like to pay for it?");

                if (!acceptOrder) {
                    _appData.DeleteOrder(newOrderID);
                    newOrder = null;
                    return BackToMenuPrompt();
                }

                _appData.CurrentUser.TakeMoney(totalPrice);
                if (_appData.CurrentUser.Account.Sum > totalPrice) {
                    newOrder.Status = OrderStatus.Paid;
                    DefaultMarkupOutput($"[bold green rapidblink]YOUR NEW ORDER[/]");
                    InternalFlashCard(CardRenderer.GetCardInfo(newOrder));
                }
                else {
                    _appData.DeleteOrder(newOrderID);
                    newOrder = null;
                }

                return BackToMenuPrompt();
            }

            return BackToMenuPrompt();
        }


        public override string Delete() {
            ClearConsole();
            ShowHeader("[bold]Delete order[/]");

            if (Tools.ValidateArray(_appData.Orders)) {
                DefaultMarkupOutput("[red bold]There are no orders to delete.[/]");
                return BackToMenuPrompt();
            }

            var choices = new List<string>();
            foreach (var order in _appData.Orders) {
                // $"({order.OrderId}) Buyer ID: {order.BuyerId}"
                choices.Add(order.ToString());
            }
            choices.Add("Cancel");

            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a order to delete:")
                    .PageSize(10)
                    .AddChoices(choices));

            if (selectedChoice == "Cancel") return BackToMenuPrompt();

            int id = Convert.ToInt32(selectedChoice.Split(')')[0].TrimStart('('));
            Order orderToDelete = _appData.Orders.FirstOrDefault(order => order.OrderId == id);

            if (orderToDelete != null) {
                bool confirm = AnsiConsole.Confirm(
                    $"Are you sure you want to delete this order ({orderToDelete.OrderId})?",
                    defaultValue: false
                );

                if (confirm) {
                    bool isDeleted = _appData.DeleteOrder(orderToDelete.OrderId);
                    if (isDeleted) {
                        DefaultMarkupOutput("[green]Order successfully deleted![/]");
                    }
                    else {
                        DefaultMarkupOutput("[red]Failed to delete order.[/]");
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
