using NOptional;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OOP_Main {
    public class SupplierData: BaseDataRepository<Supplier, int> {
        public List<Supplier> Suppliers => Items;

        public SupplierData() : base("DataStorage/suppliers.json") { }

        public void AddSupplier(Supplier supplier) => Add(supplier);

        public void AddSupplier(string name, string contactEmail, double rating) {
            int newSupplierId = Suppliers.Count > 0 ? Suppliers.Max(s => s.SupplierId) : 0;
            newSupplierId++;
            Supplier newSupplier = new Supplier(newSupplierId, name, contactEmail, rating);

            AddSupplier(newSupplier);
        }

        public bool DeleteSupplierByName(string name) {
            var supplierToDelete = this.GetSupplierByName(name);
            if (supplierToDelete.HasValue()) {
                Suppliers.Remove(supplierToDelete.Value);
                this.Save();
                return true;
            }
            return false;
        }

        public bool DeleteSupplierById(int id) => Remove(id);

        public override IOptional<Supplier> Get(int id) {
            Supplier foundSupplier = Suppliers.Find((supplier) => supplier.SupplierId == id);
            return Optional.OfNullable(foundSupplier);
        }

        public IOptional<Supplier> GetSupplierByName(string name) {
            Supplier foundSupplier = Suppliers.Find((supplier) => supplier.Name == name);
            return Optional.OfNullable(foundSupplier);
        }

        public IOptional<Supplier> GetSupplierById(int id) => this.Get(id);
    }
}
