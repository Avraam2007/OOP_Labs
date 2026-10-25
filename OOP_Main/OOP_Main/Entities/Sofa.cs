using OOP_Main.Attributes;

namespace OOP_Main.Entities {
    public class Sofa : Furniture {
        private bool isAssemble;

        [Editable("Sofa assembling")]
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

    }
}
