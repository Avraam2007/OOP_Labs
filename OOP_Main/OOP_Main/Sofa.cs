using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public class Sofa: Furniture {
        private bool isAssemble;

        public bool IsAssemble { 
            get {
                return isAssemble; 
            }
            private set {
                this.isAssemble = value;
            }
        }
        public Sofa(
            string article,
            string name,
            double price,
            double weight,
            double width,
            double length,
            double height,
            string material,
            bool isAssemble,
            int supplierId = 0
        ) : base(article, name, price, weight, width, length, height, material, supplierId) {
            this.isAssemble = isAssemble;
        }

        public override Dictionary<string, Text> ShowInfo() {
            Dictionary<string, Text> textsForRender = new Dictionary<string, Text> {
                ["header"] = new Text($"SOFA \"{this.Name}\"\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["article"] = new Text($"Article: {this.Article}\n"),
                ["price"] = new Text($"Price: {this.Price}$\n"),
                ["length"] = new Text($"Length: {this.Length} cm\n"),
                ["width"] = new Text($"Width: {this.Width} cm\n"),
                ["height"] = new Text($"Height: {this.Height} cm\n"),
                ["weight"] = new Text($"Weight: {this.Weight} kg\n"),
                ["material"] = new Text($"Material: {this.Material}\n"),
                ["isAssembled"] = new Text($"Is assembled from the store? {this.IsAssemble}\n")
            };

            return textsForRender;
        }

    }
}
