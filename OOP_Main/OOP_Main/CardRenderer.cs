using Spectre.Console;
using System.Collections.Generic;

namespace OOP_Main {
    public static class CardRenderer {
        public static Dictionary<string, Text> GetCardInfo(object item) {
            switch (item) {
                case User user:
                    return GetUserCard(user);

                case Order order:
                    return GetOrderCard(order);

                case Product product:
                    return GetProductCard(product);

                case Supplier supplier:
                    return GetSupplierCard(supplier);

                default:
                    return new Dictionary<string, Text> {
                        ["header"] = new Text(item?.ToString() ?? string.Empty)
                    };
            }
        }

        private static Dictionary<string, Text> GetUserCard(User user) {
            return new Dictionary<string, Text> {
                ["header"] = new Text($"{user.Username}\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["id"] = new Text($"ID: {user.Id}\n"),
                ["isAdmin"] = new Text($"Is admin: {user.IsAdmin}\n")
            };
        }

        private static Dictionary<string, Text> GetOrderCard(Order order) {
            var card = new Dictionary<string, Text> {
                ["header"] = new Text($"ORDER #{order.OrderId}\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["buyer"] = new Text($"Buyer ID: {order.BuyerId}\n"),
                ["total"] = new Text($"Total price: {order.GetTotalPrice()}$\n"),
                ["average"] = new Text($"Average price: {order.GetAveragePrice()}$\n"),
                ["minimal"] = new Text($"Minimal price: {order.GetMinPrice()}$\n"),
                ["maximal"] = new Text($"Maximal price: {order.GetMaxPrice()}$\n")
            };

            if (Tools.ValidateArray(order.Products)) {
                card["empty_products"] = new Text("  (No products in this order)\n", new Style(foreground: Color.Red));
                return card;
            }

            for (int i = 0; i < order.Products.Count; i++) {
                var product = order.Products[i];

                var productSpecs = GetProductCard(product);

                card[$"prod_{i}_name"] = new Text($"\n  {i + 1}. [{product.Article}] {product.Name}\n", new Style(foreground: Color.Green, decoration: Decoration.Bold));

                foreach (var spec in productSpecs) {
                    if (spec.Key == "header") continue;

                    string uniqueKey = $"prod_{i}_{spec.Key}";

                    card[uniqueKey] = spec.Value;
                }
            }

             return card;
        }

        private static Dictionary<string, Text> GetProductCard(Product product) {
            string categoryTitle = Tools.GetTypeName(product).ToUpper();
            var card = new Dictionary<string, Text> {
                ["header"] = new Text($"{categoryTitle} \"{product.Name}\"\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["article"] = new Text($"Article: {product.Article}\n"),
                ["price"] = new Text($"Price: {product.Price}$\n")
            };

            switch (product) {
                case ElectronicProduct ep:
                    card["power"] = new Text($"Power: {ep.Power} W\n");
                    card["voltage"] = new Text($"Max Voltage: {ep.MaxVoltage} V\n");
                    break;

                case Cloth cloth:
                    card["material"] = new Text($"Material: {cloth.Material}\n");
                    break;

                case Sofa sofa:
                    card["dimensions"] = new Text($"Dimensions: {sofa.Length}x{sofa.Width}x{sofa.Height} cm\n");
                    card["weight"] = new Text($"Weight: {sofa.Weight} kg\n");
                    card["material"] = new Text($"Material: {sofa.Material}\n");
                    card["assemble"] = new Text($"Is Assembled: {sofa.IsAssemble}\n");
                    break;

                case Furniture furniture:
                    card["dimensions"] = new Text($"Dimensions: {furniture.Length}x{furniture.Width}x{furniture.Height} cm\n");
                    card["weight"] = new Text($"Weight: {furniture.Weight} kg\n");
                    card["material"] = new Text($"Material: {furniture.Material}\n");
                    break;
            }

            return card;
        }

        private static Dictionary<string, Text> GetSupplierCard(Supplier supplier) {
            var card = new Dictionary<string, Text> {
                ["header"] = new Text($"SUPPLIER \"{supplier.Name}\"\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["rating"] = new Text($"Rating: {supplier.Rating}/5.0\n"),
                ["id"] = new Text($"Supplier ID: {supplier.SupplierId}\n"),
                ["reputation"] = new Text($"Reputation: {(supplier.Inspect() ? "Good" : "Bad")}\n"),
            };

            if (Tools.ValidateArray(supplier.Catalog)) {
                card["empty_products"] = new Text("  (No products that supplier can provide)\n", new Style(foreground: Color.Red));
                return card;
            }

            for (int i = 0; i < supplier.Catalog.Count; i++) {
                var product = supplier.Catalog[i];

                var productSpecs = GetProductCard(product);

                card[$"prod_{i}_name"] = new Text($"\n  {i + 1}. [{product.Article}] {product.Name}\n", new Style(foreground: Color.Green, decoration: Decoration.Bold));

                foreach (var spec in productSpecs) {
                    if (spec.Key == "header") continue;

                    string uniqueKey = $"prod_{i}_{spec.Key}";

                    card[uniqueKey] = spec.Value;
                }
            }

            return card;
        }
    }
}
