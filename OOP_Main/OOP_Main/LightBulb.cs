using System;

namespace OOP_Main {
    public class LightBulb : ElectronicPart, IQualityCheckable {
        private double brightness;
        private double temperature;
        private Wire wire;
        public double Brightness {
            private set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Brightness should be posititve");
                }
                brightness = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
            get {
                return brightness;
            }
        }
        public double Tempreature {
            private set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Temperature should be posititve (in kelvins)");
                }
                temperature = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
            get {
                return temperature;
            }
        }

        public Wire Wire { 
            get { return this.wire; } 
            set { this.wire = value; } 
        }
        public LightBulb(
            string article,
            string name,
            double price,
            int power,
            int maxVoltage,
            double brightness,
            double temperature,
            int supplierId = 0
        ) : base(article, name, price, power, maxVoltage, supplierId) {
            this.Brightness = brightness;
            this.Tempreature = temperature;
            this.Wire = new Wire("0", "Wire", 0.5, 0.5, "Copper", 0.5, 15, supplierId);
        }

        // Тут комбінований зв'язок (Композиція й агрегація)
        //public LightBulb(
        //    string article,
        //    string name,
        //    double price,
        //    int power,
        //    int maxVoltage,
        //    double brightness,
        //    double temperature,
        //    int supplierId = 0,
        //    Wire wire = null
        //) : base(article, name, price, power, maxVoltage, supplierId) {
        //    this.Brightness = brightness;
        //    this.Tempreature = temperature;
        //    if (wire != null) {
        //        this.Wire = wire;
        //    }
        //    else {
        //        this.Wire = new Wire("0", "Wire", 0.5, 0.5, "Copper", 0.5, 15, supplierId);
        //    }
        //}

        public override void ShowInfo() {
            Console.WriteLine($"" +
                $"Artile:\t\t {this.Article}\n" +
                $"Supplier ID:\t {this.GetSupplierId()}\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Price:\t\t {this.Price}$\n" +
                $"Power:\t\t {this.Power} W\n" +
                $"Max voltage:\t {this.MaxVoltage} V\n" +
                $"Brightness:\t {this.Brightness} Lm\n" +
                $"Temperature:\t {this.Tempreature} K\n" +
                $"Inspect:\t {this.Inspect()}\n");
        }

        public override int GetDeliveryDays() => 5;

        public bool Inspect() => Brightness >= 2000;
    }
}
