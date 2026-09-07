using System;

namespace OOP_Main {
    public class EnginePiston: MechanicalPart, IQualityCheckable {
        private double diameter;
        public double Diameter {
            private set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Diameter should be posititve");
                }
                diameter = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
            get {
                return diameter;
            }
        }
        public EnginePiston(
            string article,
            string name,
            double price,
            double weight,
            string material,
            double diameter,
            int supplierId = 0
        ) : base(article, name, price, weight, material, supplierId) {
            this.Diameter = diameter;
        }

        public override void ShowInfo() {
            Console.WriteLine($"" +
                $"Artile:\t\t {this.Article}\n" +
                $"Supplier ID:\t {this.GetSupplierId()}\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Price:\t\t {this.Price}$\n" +
                $"Weight:\t\t {this.Weight} mg\n" +
                $"Material:\t {this.Material}\n" +
                $"Diameter:\t {this.Diameter} cm\n" +
                $"Inspect:\t {this.Inspect()}\n");
        }

        public override int GetDeliveryDays() => 4;

        public bool Inspect() => Diameter >= 80.0 && Diameter <= 80.5;
    }
}
