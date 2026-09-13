using Spectre.Console;
using System;

namespace OOP_Main {
    public class Cloth : Product {
        private string material;
        private readonly int supplierId;
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
        public Cloth(
            string article,
            string name,
            double price,
            string material,
            int supplierId
        ) : base(article, name, price) {
            this.material = material;
            this.supplierId = supplierId;
        }

        public override void ShowInfo() {
            AnsiConsole.WriteLine($"" +
                $"Artile:\t\t {this.Article}\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Price:\t\t {this.Price}$\n" +
                $"Material:\t {this.Material}\n");
        }

        public override int GetSupplierId() => this.supplierId;

    }
}
