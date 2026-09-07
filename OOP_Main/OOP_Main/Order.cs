using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Order {
        private List<Part> parts;
        public int orderId;
        public List<Part> Parts { 
            get { return parts; }
            private set { parts = value; } 
        }

        public void AddPart(Part newPart) {
            parts.Add(newPart);
        }

        public double GetTotalPrice() {
            if (Parts.Count == 0 || Parts is null) return 0;
            double totalPrice = 0;
            foreach (Part part in Parts) { 
                totalPrice += part.Price;
            }
            return totalPrice;
        }


        public Order(int orderId, List<Part> parts) {
            this.orderId = orderId;
            Parts = parts;
        }

        public static double[] FindMinMaxPrices(List<Part> parts) {
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

        public void PrintOrder() {
            Console.WriteLine($"=====================ORDER============================\n");
            Console.WriteLine($"Order ID: {this.orderId}\n");
            foreach (var part in parts) {
                part.ShowInfo();
                Console.WriteLine();
            }
            Console.WriteLine($"Total price: {this.GetTotalPrice()}$\n");
        }
    }
}
