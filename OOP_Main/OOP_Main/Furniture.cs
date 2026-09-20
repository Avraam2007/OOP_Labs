using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Furniture : Product {
        private string material;
        private double length;
        private double width;
        private double height;
        private double weight;
        public double Weight {
            get {
                return weight;
            }
            protected set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Weight should be posititve (we don't sell antimatter)");
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
                    throw new ArgumentOutOfRangeException("Width should be positive");
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
                    throw new ArgumentOutOfRangeException("Length should be positive");
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
                    throw new ArgumentOutOfRangeException("Height should be positive");
                }
                height = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            }
        }
        public string Material {
            get {
                return material;
            }
            protected set {
                if (string.IsNullOrWhiteSpace(value)) {
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
            int supplierId = 0
        ) : base(article, name, price, supplierId) {
            this.Weight = weight;
            this.Width = width;
            this.Length = length;
            this.Height = height;
            this.Material = material;
        }
        public override int GetSupplierId() => this.SupplierId;

    }
}
