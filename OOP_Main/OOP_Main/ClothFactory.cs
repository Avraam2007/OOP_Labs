namespace OOP_Main {
    public class ClothFactory: ProductFactory {
        public override Product CreateProduct(string article, string name, double price, int supplierId, string category = "") {
            string material = DefaultTextPrompt<string>("Enter product [green]material[/]:");

            return new Cloth(article, name, price, material, supplierId);
        }
    }
}
