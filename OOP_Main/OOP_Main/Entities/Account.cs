using System;

namespace OOP_Main.Entities {
    public class Account {
        private int sum;

        public int Sum {
            get => sum;
            set {
                if (value < 0) {
                    sum = 0;
                }
                sum = value;
            } 
        }
        public Account(int sum) => this.Sum = sum;
        public void Add(int sum) => this.Sum += sum;
        public void Take(int sum) {
            if (this.sum >= sum) {
                this.sum -= sum;
                Console.WriteLine($"New transaction: -{sum}$");
            }
        }
    }
}
