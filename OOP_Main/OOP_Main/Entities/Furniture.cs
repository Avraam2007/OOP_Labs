using Newtonsoft.Json;
using OOP_Main.Attributes;
using OOP_Main.Structures;
using OOP_Main.Utilities;
using System;

namespace OOP_Main.Entities {
    public class Furniture : Product {
        private string material;
        private Dimensions _dimensions;
        private double weight;

        [Editable("Furniture weight")]
        public double Weight {
            get {
                return weight;
            }
            protected set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Weight should be posititve (we don't sell antimatter)");
                }
                weight = value.RoundNumber(3);
            }
        }

        [Editable("Furniture width")]
        public double Width {
            get {
                return _dimensions.Width;
            }
            protected set {
                _dimensions = _dimensions.WithWidth(value);
            }
        }

        [Editable("Furniture length")]
        public double Length {
            get {
                return _dimensions.Length;
            }
            protected set {
                _dimensions = _dimensions.WithLength(value);
            }
        }

        [Editable("Furniture height")]
        public double Height {
            get {
                return _dimensions.Height;
            }
            protected set {
                _dimensions = _dimensions.WithHeight(value);
            }
        }

        [Editable("Furniture material")]
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

        [JsonIgnore]
        public Dimensions Dimensions {
            get { return _dimensions; }
            protected set { _dimensions = value; }
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
            this.Dimensions = new Dimensions(width, height, length);

            this.Weight = weight;
            this.Material = material;
        }

        public override double CalculateShippingCost() {
            return Tools.RoundPrice((Weight * 0.5) + (Width * Length * Height * 0.0001));
        }
    }
}
