using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main.UI {
    public static class RecieptGenerator {
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

                    sb.AppendFormat("{0,-15} | {1,-18} | {2,7:F2}$", itemArticle, itemName, product.Price);
                    sb.AppendLine();
                }
            }

            sb.AppendLine("-------------------------------------------------");
            sb.Append("Subtotal:       ").AppendFormat("{0,28:F2}$", order.GetTotalPrice()).AppendLine();
            sb.Append("Shipping Cost:  ").AppendFormat("{0,28:F2}$", order.GetTotalShippingCost()).AppendLine();
            sb.AppendLine("[yellow]Note: you'll pay the shipping cost[/]").AppendLine("[yellow]after reicept of the order[/]");
            sb.AppendLine("=================================================");
            sb.Append("[bold]TOTAL AMOUNT:   [/]").AppendFormat("[bold]{0,28:F2}$[/]", order.GetTotalPrice() + order.GetTotalShippingCost()).AppendLine();
            sb.AppendLine("=================================================");
            sb.AppendLine("         Thank you for shopping with us!         ");

            sb.Insert(0, ">>> E-COMMERCE SYSTEM PAYMENT PROOF <<<\n\n");

            return sb.ToString();
        }

        public static void SaveReciept(string receiptText, string outputFilePath) {
            try {
                TextStorage.SaveToFile(receiptText, outputFilePath);

                AnsiConsole.MarkupLine($"[blue]Reciept was saved in file:[/] [yellow]{outputFilePath}[/]");
            }
            catch (Exception ex) {
                throw new ECommerceException($"Error with saving reciept in file '{outputFilePath}'", ex);
            }
        }
    }
}
