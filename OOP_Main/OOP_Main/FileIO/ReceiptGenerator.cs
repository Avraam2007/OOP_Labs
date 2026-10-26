using OOP_Main.Entities;
using OOP_Main.Interfaces;
using OOP_Main.Utilities;
using System;
using System.Text;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main.FileIO {
    public static class ReceiptGenerator {
        public static string BuildOrderReceipt(Order order, User buyer) {
            var sb = new StringBuilder();

            sb.AppendLine("=================================================");
            sb.AppendLine("               OFFICIAL STORE RECEIPT            ");
            sb.AppendLine("=================================================");
            sb.Append("Order ID: ").AppendLine(order.OrderId.ToString());
            sb.Append("Customer: ").AppendLine(buyer?.Username ?? "Anonymous");
            sb.Append("Date:     ").AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("-------------------------------------------------");

            sb.AppendLine(string.Format("{0,-15} | {1,-18} | {2,8}", "ARTICLE", "NAME", "PRICE"));
            sb.AppendLine("-------------------------------------------------");

            if (Tools.ValidateArray(order.Products)) {
                foreach (var product in order.Products) {
                    string itemArticle = product.Article.Length > 15 ? product.Article.Substring(0, 12) + "..." : product.Article;
                    string itemName = product.Name.Length > 18 ? product.Name.Substring(0, 15) + "..." : product.Name;

                    string formattedPrice = product.Price.GetCurrencyOutput();
                    sb.AppendFormat("{0,-15} | {1,-18} | {2,8}", itemArticle, itemName, formattedPrice);
                    sb.AppendLine();
                }
            }

            double totalAmount = order.GetTotalPrice() + order.GetTotalShippingCost();
            sb.AppendLine("-------------------------------------------------");
            sb.Append("Subtotal:       ").AppendFormat("{0,28}", order.GetTotalPrice().GetCurrencyOutput()).AppendLine();
            sb.Append("Shipping Cost:  ").AppendFormat("{0,28}", order.GetTotalShippingCost().GetCurrencyOutput()).AppendLine();
            sb.AppendLine("[yellow]Note: you'll pay the shipping cost[/]").AppendLine("[yellow]after reicept of the order[/]");
            sb.AppendLine("=================================================");
            sb.Append("[bold]TOTAL AMOUNT:   [/]").AppendFormat("[bold]{0,28}[/]", totalAmount.GetCurrencyOutput()).AppendLine();
            sb.AppendLine("=================================================");
            sb.AppendLine("         Thank you for shopping with us!         ");

            sb.Insert(0, ">>> E-COMMERCE SYSTEM PAYMENT PROOF <<<\n\n");

            return sb.ToString();
        }

        public static void SaveReceipt(string receiptText, string outputFilePath, IStorage<string> storage) {
            try {
                storage.SaveToFile(receiptText, outputFilePath);
            }
            catch (Exception ex) {
                throw new ECommerceException($"Error with saving receipt in file '{outputFilePath}'", ex);
            }
        }
    }
}
