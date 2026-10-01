namespace OOP_Main {
    internal class FurnitureFactory: ProductFactory {
        public override Product CreateProduct(string article, string name, double price, int supplierId, string category) {
            double weight = DefaultTextPrompt<double>("Enter product [green]weight (in kg)[/]:");

            double width = DefaultTextPrompt<double>("Enter product [green]width (in cm)[/]:");

            double length = DefaultTextPrompt<double>("Enter product [green]length (in cm)[/]:");

            double height = DefaultTextPrompt<double>("Enter product [green]height (in cm)[/]:");

            string mat = DefaultTextPrompt<string>("Enter product [green]material[/]:");

            if (category == "Sofa") {
                bool assemble = DefaultConfirm("Is the product can be [green]assembled[/]?");

                return new Sofa(
                    article,
                    name,
                    price,
                    weight,
                    width,
                    length,
                    height,
                    mat,
                    assemble,
                    supplierId
                );
            }
            else {
                return new Furniture(
                    article,
                    name,
                    price,
                    weight,
                    width,
                    length,
                    height,
                    mat,
                    supplierId
                );
            }
        }
    }
}
