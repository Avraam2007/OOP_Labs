using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Order: IDataAndUIBridge {
        private List<Product> products;
        public int orderId;
        public List<Product> Products {
            get { return products; }
            private set { products = value; }
        }

        public void AddPart(Product newPart) {
            this.Products.Add(newPart);
        }

        public Order(int orderId, List<Product> products) {
            this.orderId = orderId;
            Products = products;
        }

        public double GetTotalPrice() {
            if (Products.Count == 0 || Products is null) return 0;
            double totalPrice = 0;
            foreach (Product part in Products) {
                totalPrice += part.Price;
            }
            return totalPrice;
        }

        public double GetAveragePrice() {
            double totalPrice = GetTotalPrice();
            double averagePrice = totalPrice / Products.Count;
            return Math.Round(averagePrice, 2, MidpointRounding.AwayFromZero);
        }

        public double GetMinPrice() {
            double minPrice = Products[0].Price;
            for (int i = 1; i < Products.Count; i++) {
                if (Products[i].Price < minPrice) {
                    minPrice = Products[i].Price;
                }
            }
            return minPrice;
        }

        public double GetMaxPrice() {
            double maxPrice = Products[0].Price;
            for (int i = 1; i < Products.Count; i++) {
                if (Products[i].Price > maxPrice) {
                    maxPrice = Products[i].Price;
                }
            }
            return maxPrice;
        }

        public static double[] FindMinMaxPrices(List<Product> parts) {
            if (parts == null || parts.Count == 0) return new double[] { 0, 0 };

            double min = parts[0].Price;
            double max = parts[0].Price;

            foreach (var part in parts) {
                double price = part.Price;
                if (price < min) min = price;
                if (price > max) max = price;
            }

            return new double[] { min, max };
        }

        public List<T> Reverse<T>(List<T> arr) {
            for (int i = 0; i < arr.Count / 2; i++) {
                T tmp = arr[i];
                arr[i] = arr[arr.Count - 1 - i];
                arr[arr.Count - 1 - i] = tmp;
            }
            return arr;
        }

        public List<T> RemoveDuplicates<T>(List<T> arr) {

            List<T> ans = new List<T>();

            // traverse each element in the array
            for (int i = 0; i < arr.Count; i++) {
                int cnt = 0;

                // check if element is already added to result
                foreach (var it in ans) {
                    if (arr[i].Equals(it)) {
                        cnt++;
                        break;
                    }
                }

                // if already added, skip checking again
                if (cnt > 0) continue;

                // check if current element appears again 
                // in the rest of the array
                for (int j = i + 1; j < arr.Count; j++) {
                    if (arr[i].Equals(arr[j])) {
                        cnt++;
                        break;
                    }
                }

                // if duplicate found, delete it
                if (cnt > 0) continue;
                else ans.Add(arr[i]);
            }

            return ans;
        }

        public Dictionary<string, Text> ShowInfo() {
            Dictionary<string, Text> textsForRender = new Dictionary<string, Text> {
                ["header"] = new Text($"ORDER #{this.orderId}\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["total"] = new Text($"Total price: {this.GetTotalPrice()}$\n"),
                ["average"] = new Text($"Average price: {this.GetAveragePrice()}$\n"),
                ["minimal"] = new Text($"Minimal price: {this.GetMinPrice()}$\n"),
                ["maximal"] = new Text($"Maximal price: {this.GetMinPrice()}$\n")
            };

            if (this.Products == null || this.Products.Count == 0) {
                textsForRender["empty_products"] = new Text("  (No products in this order)\n", new Style(foreground: Color.Red));
                return textsForRender;
            }

            for (int i = 0; i < this.Products.Count; i++) {
                var product = this.Products[i];

                var productSpecs = product.ShowInfo();

                textsForRender[$"prod_{i}_name"] = new Text($"\n  {i + 1}. [{product.Article}] {product.Name}\n", new Style(foreground: Color.Green, decoration: Decoration.Bold));

                foreach (var spec in productSpecs) {
                    if (spec.Key == "header") continue;

                    string uniqueKey = $"prod_{i}_{spec.Key}";

                    textsForRender[uniqueKey] = spec.Value;
                }
            }

            return textsForRender;
        }
    }
}
