using Spectre.Console;
using System;

namespace OOP_Main {
    public class Furniture : Product {
        private string material;
        private double length;
        private double width;
        private double height;
        private double weight;
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
        public double Width {
            get {
                return width;
            }
            protected set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Width should be posititve");
                }
                width = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
        }
        public double Length {
            get {
                return length;
            }
            protected set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Length should be posititve");
                }
                length = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
        }
        public double Height {
            get {
                return height;
            }
            protected set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Height should be posititve");
                }
                height = Math.Round(value, 3, MidpointRounding.AwayFromZero);
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
        public Furniture(
            string article,
            string name,
            double price,
            double weight,
            double width,
            double length,
            double height,
            string material,
            int supplierId
        ) : base(article, name, price) {
            this.Weight = weight;
            this.Width = width;
            this.Length = length;
            this.Height = height;
            this.Material = material;
            this.supplierId = supplierId;
        }

        public override void ShowInfo() {
            AnsiConsole.MarkupLine($"" +
                $"Artile:\t\t {this.Article}\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Price:\t\t {this.Price}$\n" +
                $"Length:\t\t {this.Length} cm\n" +
                $"Width:\t\t {this.Width} cm\n" +
                $"Height:\t\t {this.Height} cm\n" +
                $"Weight:\t\t {this.Weight} kg\n" +
                $"Material:\t {this.Material}\n");
        }

        public override int GetSupplierId() => this.supplierId;

    }
}
