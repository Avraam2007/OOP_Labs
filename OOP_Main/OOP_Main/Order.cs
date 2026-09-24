using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Order {
        private int orderId;
        public int OrderId { get => orderId; private set { orderId = value; } }
        public List<Product> Products { get; private set; }
        public int BuyerId { get; private set; }

        public void AddProduct(Product newProduct) {
            this.Products.Add(newProduct);
        }

        [JsonConstructor]
        public Order(int orderId, int buyerId, List<Product> products) {
            this.OrderId = orderId;
            this.BuyerId = buyerId;
            Products = products;
        }

        public Order(int orderId, int buyerId) {
            this.OrderId = orderId;
            this.BuyerId = buyerId;
            Products = new List<Product>();
        }

        public double GetTotalPrice() {
            if (Tools.ValidateArray(Products)) return 0;
            double totalPrice = 0;
            foreach (Product part in Products) {
                totalPrice += part.Price;
            }
            return totalPrice;
        }

        public double GetAveragePrice() {
            if (Tools.ValidateArray(Products)) return 0;
            double totalPrice = GetTotalPrice();
            double averagePrice = totalPrice / Products.Count;
            return Math.Round(averagePrice, 2, MidpointRounding.AwayFromZero);
        }

        public double GetMinPrice() {
            if (Tools.ValidateArray(Products)) return 0;
            double minPrice = Products[0].Price;
            for (int i = 1; i < Products.Count; i++) {
                if (Products[i].Price < minPrice) {
                    minPrice = Products[i].Price;
                }
            }
            return minPrice;
        }

        public double GetMaxPrice() {
            if (Tools.ValidateArray(Products)) return 0;
            double maxPrice = Products[0].Price;
            for (int i = 1; i < Products.Count; i++) {
                if (Products[i].Price > maxPrice) {
                    maxPrice = Products[i].Price;
                }
            }
            return maxPrice;
        }

        public double[] FindMinMaxPrices(List<Product> parts) {
            if (Tools.ValidateArray(parts)) return new double[] { 0, 0 };

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
            if (Tools.ValidateArray(arr)) return null;
            for (int i = 0; i < arr.Count / 2; i++) {
                T tmp = arr[i];
                arr[i] = arr[arr.Count - 1 - i];
                arr[arr.Count - 1 - i] = tmp;
            }
            return arr;
        }

        public List<T> RemoveDuplicates<T>(List<T> arr) {
            if (Tools.ValidateArray(arr)) return null;

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

        public override bool Equals(object obj) {
            if (obj is Order other) return this.orderId == other.orderId;
            return false;
        }
        public override int GetHashCode() => OrderId.GetHashCode();
        public override string ToString() => $"[Order] {OrderId} (From: {BuyerId})";
    }
}
