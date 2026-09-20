using Spectre.Console;
using System.Collections.Generic;

namespace OOP_Main {
    public class ProductScreen: UIHelper {
        private readonly Data _appData;
        public ProductScreen(Data appData) {
            _appData = appData;
        }

        public string ShowProductsScreen() => ShowListScreen("Products", _appData.Products);

        public void AddingProduct(Product newProduct, string supplierChoice) {
            _appData.AddProduct(newProduct);
            _appData.GetSupplierByName(supplierChoice).AddProductToCatalog(newProduct);
            AnsiConsole.MarkupLine($"[green bold]New product is created! You can check it on \"{GetEnumDescription(MenuOption.ShowProducts)}\" screen.[/]");
        }

        public string AddProductScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Adding product[/]");

            if (Tools.ValidateArray(_appData.Suppliers)) {
                AnsiConsole.MarkupLine("[red bold]No suppliers found. Call the admin to fix this issue[/]");
                return BackToMenuPrompt();
            }

            var prompt = new SelectionPrompt<string>()
                .Title("Select product [green]category[/]")
                .PageSize(15)
                .AddChoiceGroup("Electronic product", new[]
                {
                    "Electronic product"
                })
                .AddChoiceGroup("Cloth", new[]
                {
                    "Cloth"
                })
                .AddChoiceGroup("Furniture", new[]
                {
                    "Sofa", "Other furniture"
                });
            var categoryChoice = AnsiConsole.Prompt(prompt);

            AnsiConsole.Clear();
            ShowHeader("[bold]Adding product[/]");

            string articleChoice = DefaultTextPrompt<string>("Enter product [green]article[/]:");

            string nameChoice = DefaultTextPrompt<string>("Enter product [green]name[/]:");

            double priceChoice = DefaultTextPrompt<double>("Enter product [green]price (in dollars)[/]:");

            List<string> suppliers = new List<string>();
            foreach (var supplier in _appData.Suppliers) {
                suppliers.Add(supplier.Name);
            }
            var supplierPrompt = new SelectionPrompt<string>()
                .Title("Select product [green]supplier[/]")
                .PageSize(15)
                .AddChoices(suppliers);

            string supplierChoice = AnsiConsole.Prompt(supplierPrompt);

            int supplierId = _appData.GetSupplierByName(supplierChoice).SupplierId;

            switch (categoryChoice) {
                case "Electronic product":
                    int powerChoice = DefaultTextPrompt<int>("Enter product [green]power (in watts)[/]:");
                    int maxVoltageChoice = DefaultTextPrompt<int>("Enter product [green]max voltage (in volts)[/]:");

                    var newElectronicProduct = new ElectronicProduct(
                        articleChoice,
                        nameChoice,
                        priceChoice,
                        powerChoice,
                        maxVoltageChoice,
                        supplierId
                    );
                    AddingProduct(newElectronicProduct, supplierChoice);
                    break;

                case "Cloth":
                    string materialChoice = DefaultTextPrompt<string>("Enter product [green]material[/]:");

                    var newCloth = new Cloth(
                        articleChoice,
                        nameChoice,
                        priceChoice,
                        materialChoice,
                        supplierId
                    );
                    AddingProduct(newCloth, supplierChoice);
                    break;

                case "Sofa":
                case "Other furniture":
                    double weight = DefaultTextPrompt<double>("Enter product [green]weight (in kg)[/]:");

                    double width = DefaultTextPrompt<double>("Enter product [green]width (in cm)[/]:");

                    double length = DefaultTextPrompt<double>("Enter product [green]length (in cm)[/]:");

                    double height = DefaultTextPrompt<double>("Enter product [green]height (in cm)[/]:");

                    string mat = DefaultTextPrompt<string>("Enter product [green]material[/]:");

                    if (categoryChoice == "Sofa") {
                        bool assemble = AnsiConsole.Confirm("Is the product can be [green]assembled[/]?");

                        var newSofa = new Sofa(
                            articleChoice,
                            nameChoice,
                            priceChoice,
                            weight,
                            width,
                            length,
                            height,
                            mat,
                            assemble,
                            supplierId
                        );
                        AddingProduct(newSofa, supplierChoice);
                    }
                    else {
                        var newFurniture = new Furniture(
                            articleChoice,
                            nameChoice,
                            priceChoice,
                            weight,
                            width,
                            length,
                            height,
                            mat,
                            supplierId
                        );
                        AddingProduct(newFurniture, supplierChoice);
                    }
                    break;
            }

            return BackToMenuPrompt();
        }
    }
}
