using System;

namespace OOP_Main {
    public class ElectricMotor : ElectronicPart, IQualityCheckable {
        private double rotationFrequency;
        private double energy_conversion_efficiency;
        public double RotationFrequency {
            private set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Rotation frequency should be posititve");
                }
                rotationFrequency = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
            get {
                return rotationFrequency;
            }
        }
        public double ECE {
            private set {
                if (value <= 0 || value >= 1) {
                    throw new ArgumentOutOfRangeException("ECE should range between 0 and 1");
                }
                energy_conversion_efficiency = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
            get {
                return energy_conversion_efficiency;
            }
        }
        public ElectricMotor(
            string article,
            string name,
            double price,
            int power,
            int maxVoltage,
            double rotationFrequency,
            double ECE,
            int supplierId = 0
        ) : base(article, name, price, power, maxVoltage, supplierId) {
            this.RotationFrequency = rotationFrequency;
            this.ECE = ECE;
        }

        public override void ShowInfo() {
            Console.WriteLine($"" +
                $"Artile:\t\t\t\t {this.Article}\n" +
                $"Supplier ID:\t\t\t {this.GetSupplierId()}\n" +
                $"Name:\t\t\t\t {this.Name}\n" +
                $"Price:\t\t\t\t {this.Price}$\n" +
                $"Power:\t\t\t\t {this.Power} W\n" +
                $"Max voltage:\t\t\t {this.MaxVoltage} V\n" +
                $"Rotation frequency:\t\t {this.RotationFrequency} rotattions/s\n" +
                $"Energy conversion efficiency:\t {this.ECE}\n" +
                $"Inspect:\t\t\t {this.Inspect()}\n");
        }

        public override int GetDeliveryDays() => 10;

        public bool Inspect() => ECE >= 0.34 && ECE < 1;
    }
}
