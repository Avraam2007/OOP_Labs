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
                sum = Math.Round(value, 2, MidpointRounding.AwayFromZero);
            } 
        }
        public Account(double sum) => this.Sum = sum;
        public void Add(double sum) {
            if (sum < 0) {
                taken?.Invoke($"[bold red]Adding money should be positive[/]");
            }
            else if (sum > 1000) {
                taken?.Invoke($"[bold red]Too much money. You should better send the money by parts.[/]");
            }
            else {
                this.Sum += sum;
            }
        }
        public void Take(double sum) {
            if (this.sum >= sum) {
                this.sum -= sum;
                taken?.Invoke($"[bold green]New transaction: -{sum} $[/]");
            }
            else {
                taken?.Invoke($"[bold red]Not enough money. Balance: {this.sum} $.[/]");
            }

        }
    }
}
