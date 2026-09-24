using System;

namespace OOP_Main {
    public class Cloth : Product {
        private string material;
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
        ) : base(article, name, price, supplierId) {
            this.material = material;
        }
    }
}
