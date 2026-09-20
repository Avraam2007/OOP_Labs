using Spectre.Console;
using System.Collections.Generic;

namespace OOP_Main {
    public class ElectronicProduct : Product {
        private int power;
        private int maxVoltage;
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
        ) : base(article, name, price, supplierId) {
            this.Power = power;
            this.MaxVoltage = maxVoltage;
        }

        public override int GetSupplierId() => this.SupplierId;

    }
}
