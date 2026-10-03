using System;

namespace OOP_Main.Entities {
    public delegate void AccountHandler(string message);
    public class Account {
        private double sum;
        AccountHandler taken;
        public void RegisterHandler(AccountHandler del) {
            taken = del;
        }
        public double Sum {
            get => sum;
            set {
                if (value < 0) {
                    sum = 0;
                }
                sum = value;
            } 
        }
        public Account(double sum) => this.Sum = sum;
        public void Add(double sum) => this.Sum += sum;
        public void Take(double sum) {
            if (this.sum >= sum) {
                this.sum -= sum;
                taken?.Invoke($"New transaction: -{sum} $");
            }
            else {
                taken?.Invoke($"Not enough money. Balance: {this.sum} $.");
            }

        }
    }
}
