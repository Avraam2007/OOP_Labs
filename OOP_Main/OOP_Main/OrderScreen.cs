using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class OrderScreen: UIHelper {
        private readonly Data _appData;
        public OrderScreen(Data appData) {
            _appData = appData;
        }

        public string ShowOrdersScreen() {
            if (_appData.CurrentUser == null) {
                AnsiConsole.MarkupLine($"[red bold]The access is forbidden[/]");
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

        public string CreateOrderScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Creating order[/]");

            if (Tools.ValidateArray(_appData.Products)) {
                AnsiConsole.MarkupLine("[red bold]No products found. Call the admin to fix this issue.[/]");
                return BackToMenuPrompt();
            }

            const string exitOption = "Exit";
            Order currentOrder = new Order(new Random().Next(100, 999), _appData.CurrentUser.Id);

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
                _appData.AddOrder(currentOrder);
                AnsiConsole.MarkupLine($"[bold green rapidblink]YOUR NEW ORDER[/]");
                InternalFlashCard(CardRenderer.GetCardInfo(currentOrder));
            }

            return BackToMenuPrompt();
        }


        public string DeleteOrderScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Delete order[/]");

            if (Tools.ValidateArray(_appData.Suppliers)) {
                AnsiConsole.MarkupLine("[red bold]There are no orders to delete.[/]");
                return BackToMenuPrompt();
            }

            var choices = new List<string>();
            foreach (var order in _appData.Orders) {
                choices.Add($"({order.OrderId}) Buyer ID: {order.BuyerId}");
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
                        AnsiConsole.MarkupLine("[green]Order successfully deleted![/]");
                    }
                    else {
                        AnsiConsole.MarkupLine("[red]Failed to delete order.[/]");
                    }
                }
                else {
                    AnsiConsole.MarkupLine("[yellow]Deleting cancelled.[/]");
                }
            }

            return BackToMenuPrompt();
        }
    }
}
