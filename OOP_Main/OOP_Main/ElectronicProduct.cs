using Spectre.Console;
using System;
using System.Collections.Generic;

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

        public override Dictionary<string, Text> ShowInfo() {
            Dictionary<string, Text> textsForRender = new Dictionary<string, Text> {
                ["header"] = new Text($"ELECTRONIC PRODUCT \"{this.Name}\"\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["article"] = new Text($"Article: {this.Article}\n"),
                ["price"] = new Text($"Price: {this.Price}$\n"),
                ["power"] = new Text($"Power: {this.Power} W\n"),
                ["maxVoltage"] = new Text($"Max voltage: {this.MaxVoltage} V\n")
            };

            return textsForRender;
        }

        public override int GetSupplierId() => this.supplierId;

    }
}
