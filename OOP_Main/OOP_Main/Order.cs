using System;
using System.Collections.Generic;
using System.Linq;

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

        public Order(int orderId, List<Part> parts) {
            this.orderId = orderId;
            Parts = parts;
        }

        public double GetTotalPrice() {
            if (Parts.Count == 0 || Parts is null) return 0;
            double totalPrice = 0;
            foreach (Part part in Parts) {
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

        public bool EvenOrOdd(int value) {
            return value % 2 == 0;
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

        public int Factorial(int n) {
            int res = 1, i;
            for (i = 2; i <= n; i++)
                res *= i;
            return res;
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

        public List<int> Fibonacci(int n) {
            List<int> res = new List<int>();
            int first = 0;
            int second = 1;

            for (int i = 0; i < n; i++) {
                res.Add(first);

                int next = first + second;
                first = second;
                second = next;
            }
            return res;
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

        public List<int> EvenAndOddArr(List<int> arr) {
            List<int> res = new List<int> { 0, 0 };
            foreach (var item in arr) {
                if (EvenOrOdd(item)) {
                    res[0]++;
                }
                else {
                    res[1]++;
                }
            }
            return res;
        }

        public List<int> BubbleSort(List<int> array) {
            int len = array.Count;
            for (int i = 1; i < len; i++) {
                for (int j = 0; j < len - i; j++) {
                    if (array[j] > array[j + 1]) {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }

            return array;
        }

        public void PrintOrder() {
            Console.WriteLine($"=====================ORDER============================\n");
            Console.WriteLine($"Order ID: {this.orderId}\n");
            foreach (var part in parts) {
                part.ShowInfo();
                Console.WriteLine();
            }
            Console.WriteLine($"========STATS========\n");
            Console.WriteLine($"Total price: {this.GetTotalPrice()}$\n");
            Console.WriteLine($"Average price: {this.GetAveragePrice()}$\n");
            Console.WriteLine($"Minimal price: {this.GetMinPrice()}$\n");
            Console.WriteLine($"Maximal price: {this.GetMaxPrice()}$\n");
            Console.WriteLine($"Is total amount of products in order even?: {(this.EvenOrOdd(Parts.Count) ? "Yes" : "No")}\n");
            Console.WriteLine($"5! = {this.Factorial(5)}\n");
            Console.Write("First 10 Fibonacchi numbers: ");
            this.PrintArray<int>(this.Fibonacci(10));
            Console.WriteLine();
            Console.Write("Reversed array: ");
            this.PrintArray<int>(
                this.EvenAndOddArr(
                        this.Fibonacci(10)
                )
            );
            Console.WriteLine();
            Console.Write("Array without repeats: ");
            this.PrintArray<int>(
                this.RemoveDuplicates<int>(
                        this.Fibonacci(10)
                )
            );
            Console.WriteLine();
            Console.Write("Amount of even and odd numbers: ");
            this.PrintArray<int>(
                this.EvenAndOddArr(
                        this.Fibonacci(10)
                )
            );
            Console.WriteLine();
            Console.Write("Sorted array from { 2, 6, 3, 0, 95, 64, -1, 8 }: ");
            this.PrintArray<int>(
                this.BubbleSort(
                        new List<int> { 2, 6, 3, 0, 95, 64, -1, 8 }
                )
            );
            Console.WriteLine();
        }
    }
}
