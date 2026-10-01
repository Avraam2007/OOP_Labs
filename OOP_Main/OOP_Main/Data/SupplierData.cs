using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class SupplierData: IBridgeJSON {
        public static readonly string SuppliersFilePath = "DataStorage/suppliers.json";

        public List<Supplier> Suppliers { get; private set; } = new List<Supplier>();

        public void Load() {
            Suppliers = JsonStorage.LoadFromFile<List<Supplier>>(SuppliersFilePath);
        }

        public void Save() {
            JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);
        }

        public void AddSupplier(Supplier supplier) {
            Suppliers.Add(supplier);
            JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);
        }

        public void AddSupplier(string name, string contactEmail, double rating) {
            int newSupplierId = Suppliers.Count > 0 ? Suppliers.Max(s => s.SupplierId) : 0;
            newSupplierId++;
            Supplier newSupplier = new Supplier(newSupplierId, name, contactEmail, rating);

            AddSupplier(newSupplier);
        }

        public bool DeleteSupplierByName(string name) {
            Supplier supplierToDelete = this.GetSupplierByName(name);
            if (supplierToDelete != null) {
                Suppliers.Remove(supplierToDelete);
                JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);
                return true;
            }
            return false;
        }

        public Supplier GetSupplierByName(string name) {
            Supplier foundSupplier = Suppliers.Find((supplier) => supplier.Name == name);
            return foundSupplier;
        }
    }
}
