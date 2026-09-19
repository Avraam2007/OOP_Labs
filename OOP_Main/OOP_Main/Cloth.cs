using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Cloth : Product {
        private string material;
        private readonly int supplierId;
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

        public override Dictionary<string, Text> ShowInfo() {
            Dictionary<string, Text> textsForRender = new Dictionary<string, Text> {
                ["header"] = new Text($"CLOTH \"{this.Name}\"\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["article"] = new Text($"Article: {this.Article}\n"),
                ["price"] = new Text($"Price: {this.Price}$\n"),
                ["material"] = new Text($"Material: {this.Material}\n")
            };

            return textsForRender;
        }

        public override int GetSupplierId() => this.supplierId;

    }
}
