using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public class Wire : MechanicalPart, IQualityCheckable {
        private double diameter;
        private double length;
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

        public double Length {
            private set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Length should be posititve");
                }
                length = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
            get {
                return length;
            }
        }
        public Wire(
            string article,
            string name,
            double price,
            double weight,
            string material,
            double diameter,
            double length,
            int supplierId = 0
        ) : base(article, name, price, weight, material, supplierId) {
            this.Diameter = diameter;
            this.Length = length;
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
                $"Length:\t {this.Length} cm\n" +
                $"Inspect:\t {this.Inspect()}\n");
        }

        public override int GetDeliveryDays() => 4;

        public bool Inspect() => (Diameter >= 0.1 && Diameter <= 0.5) && (Material == "Copper" || Material == "Aluminium");
    }
}
