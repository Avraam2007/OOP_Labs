using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public class LightBulb: ElectronicPart, IQualityCheckable {
        private double brightness;
        private double temperature;
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
        }
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
