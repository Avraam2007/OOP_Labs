using System;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main.Entities {
    //public delegate void AccountHandler(string message);
    public class Account {
        private double sum;
        //AccountHandler taken;
        //public void RegisterHandler(AccountHandler del) {
        //    taken = del;
        //}
        public double Sum {
            get => sum;
            set {
                if (value < 0) {
                    sum = 0;
                }
                sum = Math.Round(value, 2, MidpointRounding.AwayFromZero);
            } 
        }
        public Account(double sum) => this.Sum = sum;
        public void Add(double amount) {
            if (amount < 0) {
                throw new ArgumentOutOfRangeException(nameof(amount), "Adding money should be positive");
            }
            else if (amount > 1000) {
                throw new ArgumentOutOfRangeException(nameof(amount), "Too much money. You should better send the money by parts.");
            }
            else {
                this.Sum += amount;
            }
        }
        public void Take(double amount) {
            if (amount <= 0) {
                throw new ArgumentOutOfRangeException(nameof(amount), "Taking money should be positive");
            }
            if (this.Sum < amount) {
                throw new InsufficientFundsException(sum, this.Sum);
            }
            else {
                this.Sum -= amount;
            }
        }
    }
}
