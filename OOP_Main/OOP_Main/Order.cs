using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Order {
        private List<Product> parts;
        public int orderId;
        public List<Product> Parts {
            get { return parts; }
            private set { parts = value; }
        }

        public void AddPart(Product newPart) {
            parts.Add(newPart);
        }

        public Order(int orderId, List<Product> parts) {
            this.orderId = orderId;
            Parts = parts;
        }

        public double GetTotalPrice() {
            if (Parts.Count == 0 || Parts is null) return 0;
            double totalPrice = 0;
            foreach (Product part in Parts) {
                totalPrice += part.Price;
            }
            return totalPrice;
        }

        public double GetAveragePrice() {
            double totalPrice = GetTotalPrice();
            double averagePrice = totalPrice / Parts.Count;
            return Math.Round(averagePrice, 2, MidpointRounding.AwayFromZero);
        }

        public double GetMinPrice() {
            double minPrice = Parts[0].Price;
            for (int i = 1; i < Parts.Count; i++) {
                if (Parts[i].Price < minPrice) {
                    minPrice = Parts[i].Price;
                }
            }
            return minPrice;
        }

        public double GetMaxPrice() {
            double maxPrice = Parts[0].Price;
            for (int i = 1; i < Parts.Count; i++) {
                if (Parts[i].Price > maxPrice) {
                    maxPrice = Parts[i].Price;
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


        public void PrintArray<T>(List<T> arr) {
            if (arr == null || arr.Count == 0) return;
            Console.Write(string.Join(", ", arr));
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

        public void PrintOrder() {
            AnsiConsole.MarkupLine($"=====================ORDER============================\n");
            AnsiConsole.MarkupLine($"Order ID: {this.orderId}\n");
            foreach (var part in parts) {
                part.ShowInfo();
                AnsiConsole.WriteLine();
            }
            AnsiConsole.MarkupLine($"========STATS========\n");
            AnsiConsole.MarkupLine($"Total price: {this.GetTotalPrice()}$\n");
            AnsiConsole.MarkupLine($"Average price: {this.GetAveragePrice()}$\n");
            AnsiConsole.MarkupLine($"Minimal price: {this.GetMinPrice()}$\n");
            AnsiConsole.MarkupLine($"Maximal price: {this.GetMaxPrice()}$\n");
            AnsiConsole.WriteLine();
        }
    }
}
