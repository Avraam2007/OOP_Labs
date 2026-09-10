using System;

namespace OOP_Main {
    public class MechanicalPart : Part {
        private double weight;
        private string material;
        private readonly int supplierId;
        public double Weight {
            get {
                return weight;
            }
            protected set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Weight should be posititve (we don't sale antimatter)");
                }
                weight = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
        }
        public string Material {
            get {
                return material;
            }
            protected set {
                if (value.Trim().Length == 0) {
                    throw new ArgumentException("Material name shouldn\'t be empty");
                }
                material = value;
            }
        }
        public MechanicalPart(
            string article,
            string name,
            double price,
            double weight,
            string material,
            int supplierId
        ) : base(article, name, price) {
            this.Weight = weight;
            this.material = material;
            this.supplierId = supplierId;
        }

        public override void ShowInfo() {
            Console.WriteLine($"" +
                $"Artile:\t\t {this.Article}\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Price:\t\t {this.Price}$\n" +
                $"Weight:\t\t {this.Weight} mg\n" +
                $"Material:\t {this.Material}\n");
        }

        public override int GetSupplierId() => this.supplierId;

    }
}
