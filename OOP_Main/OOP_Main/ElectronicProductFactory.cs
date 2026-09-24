namespace OOP_Main {
    public class ElectronicProductFactory : ProductFactory {
        public override Product CreateProduct(string article, string name, double price, int supplierId, string category = "") {
            int power = DefaultTextPrompt<int>("Enter product [green]power (in watts)[/]:");
            int maxVoltage = DefaultTextPrompt<int>("Enter product [green]max voltage (in volts)[/]:");

            return new ElectronicProduct(article, name, price, power, maxVoltage, supplierId);
        }
    }
}
