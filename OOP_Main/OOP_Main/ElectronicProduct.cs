using Spectre.Console;
using System;

namespace OOP_Main {
    public class ElectronicProduct : Product {
        private int power;
        private int maxVoltage;
        private readonly int supplierId;
        public int Power {
            get {
                return power;
            }
            protected set {
                power = value;
            }
        }
        public int MaxVoltage {
            get {
                return maxVoltage;
            }
            protected set {
                maxVoltage = value;
            }
        }
        public ElectronicProduct(
            string article,
            string name,
            double price,
            int power,
            int maxVoltage,
            int supplierId
        ) : base(article, name, price) {
            this.Power = power;
            this.MaxVoltage = maxVoltage;
            this.supplierId = supplierId;
        }

        public override void ShowInfo() {
            AnsiConsole.MarkupLine($"" +
                $"Artile:\t\t {this.Article}\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Price:\t\t {this.Price}$\n" +
                $"Power:\t\t {this.Power} W\n" +
                $"Max voltage:\t {this.MaxVoltage} V\n");
        }

        public override int GetSupplierId() => this.supplierId;

    }
}
