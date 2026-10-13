using NOptional;
using OOP_Main.UI;
using OOP_Main.Utilities;
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
                header: "Orders",
                items: CheckIfUserIsAdmin(_appData.CurrentUser) ?
                _appData.Orders :
                _appData.GetOrdersFromUser(_appData.CurrentUser.Value.Id),
                extraErrorMessage: "You can create it."
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

            return BackToMenuPrompt();
        }

        public string Delete() => DeleteObjectScreen<Order, int>(
            _appData.Orders,
            _appData.DeleteOrder,
            order => $"[red]\"({order.OrderId}) from {order.BuyerId}\"[/]"
        );
    }
}
