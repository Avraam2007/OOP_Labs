using NOptional;
using OOP_Main.Comparers;
using OOP_Main.Structures;
using OOP_Main.UI;
using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main.Screens {
    public class ProductScreen: BaseScreen, IScreen {
        public ProductScreen(Data appData): base(appData) { }

        public string Show() {
            ShowHeader("Products");

            var sortOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose sorting option:")
                    .AddChoices("By article (default)", "By price (from the cheapest)", "By name (A-Z)")
            );

            List<Product> productsToSort = new List<Product>(_appData.Products);

            switch (sortOption) {
                case "By article (default)":
                    productsToSort.Sort();
                    break;
                case "By price (from the cheapest)":
                    productsToSort.Sort(new ProductPriceComparer());
                    break;
                case "By name (A-Z)":
                    productsToSort.Sort(new ProductNameComparer());
                    break;
            }
            return ShowListScreen("Products", productsToSort);
        }

        private void AddingProduct(Product newProduct, string supplierChoice, bool isDiscount, bool isExtendedWarranty) {
            Product changedProduct = newProduct;
            if (isDiscount) {
                changedProduct = new DiscountDecorator(newProduct, discountPercent: 20);
            }
            if (isExtendedWarranty) {
                changedProduct = new ExtendedWarrantyDecorator(newProduct, years: 1);
            }
            try {
                _appData.AddProduct(changedProduct);
                _appData.GetSupplierByName(supplierChoice).Value.AddProductToCatalog(changedProduct);
                DefaultSuccessOutput($"New product is created! You can check it on \"{GetEnumDescription(MenuOption.ShowProducts)}\" screen.");
            }
            catch (ArgumentException ex) {
                DefaultErrorOutput(ex.Message);
            }
        }

        private ProductBundle FillBundle(Product bundle) {
            ShowHeader("Filling bundle");

            if (!(bundle is ProductBundle targetBundle)) {
                throw new ECommerceException("Provided product is not a product bundle.");
            }

            if (Tools.ValidateArray(_appData.Products)) {
                throw new ECommerceException("No products found. Call the admin to fix this issue.");
            }

            const string exitOption = "Exit";

            var productNames = _appData.Products.Select(p => p.Name).ToList();

            productNames.Add(exitOption);

            bool isChosenExit = false;

            while (!isChosenExit) {
                var prompt = new SelectionPrompt<string>()
                    .Title("Select [green]product[/] to add into bundle")
                    .PageSize(15)
                    .AddChoices(productNames);
                var productChoice = AnsiConsole.Prompt(prompt);

                if (productChoice == exitOption) {
                    isChosenExit = true;
                    continue;
                }

                IOptional<Product> foundProduct = _appData.GetProductByName(productChoice);
                if (foundProduct.HasValue()) {
                    targetBundle.AddProduct(foundProduct.Value);
                }
                else {
                    DefaultMarkupOutput($"Sorry, we didn't found this product. Try again");
                }
                productNames.Remove(productChoice);
                ShowHeader("Filling bundle");
            }

            return targetBundle.Items.Count > 0 ? targetBundle : null;
        }

        public string Create() {
            ShowHeader("Adding product");

            if (Tools.ValidateArray(_appData.Suppliers)) {
                DefaultErrorOutput("No suppliers found. Call the admin to fix this issue");
                return BackToMenuPrompt();
            }

            var factories = new Dictionary<string, ProductFactory> {
                { "Electronic product", new ElectronicProductFactory() },
                { "Cloth", new ClothFactory() },
                { "Furniture", new FurnitureFactory() },
                { "Sofa", new FurnitureFactory() },
                { "Bundle", new BundleFactory() },
            };       

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
                })
                .AddChoices("Bundle");
            var categoryChoice = AnsiConsole.Prompt(prompt);

            Type productType = typeof(Product);
            switch (categoryChoice) {
                case "Electronic product": productType = typeof(ElectronicProduct); break;
                case "Cloth": productType = typeof(Cloth); break;
                case "Sofa": productType = typeof(Sofa); break;
                case "Other furniture": productType = typeof(Furniture); break;
                case "Bundle": productType = typeof(ProductBundle); break;
            }

            string autoGeneratedArticle = _appData.GenerateNextProductArticle(productType);

            ShowHeader("Adding product");

            string nameChoice = DefaultTextPrompt<string>("Enter product [green]name[/]:");

            double priceChoice = DefaultTextPrompt<double>("Enter product [green]price (in dollars)[/]:");

            bool discountChoice = DefaultConfirm("Would you like to add the [green]discount?[/]:");

            bool warrantyChoice = DefaultConfirm("Would you like to add the [green]warranty?[/]:");

            var suppliers = _appData.Suppliers.Select(s => s.Name).ToList();

            var supplierPrompt = new SelectionPrompt<string>()
                .Title("Select product [green]supplier[/]")
                .PageSize(15)
                .AddChoices(suppliers);

            string supplierChoice = AnsiConsole.Prompt(supplierPrompt);

            int supplierId = _appData.GetSupplierByName(supplierChoice).GetValueOrElseThrow().SupplierId;

            switch (categoryChoice) {
                case "Electronic product":
                    Product newElectronicProduct = factories["Electronic product"].CreateProduct(autoGeneratedArticle, nameChoice, priceChoice, supplierId);

                    AddingProduct(newElectronicProduct, supplierChoice, discountChoice, warrantyChoice);
                    break;

                case "Cloth":
                    Product newCloth = factories["Cloth"].CreateProduct(autoGeneratedArticle, nameChoice, priceChoice, supplierId);

                    AddingProduct(newCloth, supplierChoice, discountChoice, warrantyChoice);
                    break;

                case "Bundle":
                    Product newBundle = factories["Bundle"].CreateProduct(autoGeneratedArticle, nameChoice, priceChoice, supplierId);

                    try {
                        newBundle = this.FillBundle(newBundle);

                        AddingProduct(newBundle, supplierChoice, discountChoice, warrantyChoice);
                    }
                    catch(ECommerceException ex) {
                        DefaultErrorOutput(ex.Message);
                    }
                    break;

                case "Sofa":
                case "Other furniture":
                    if (categoryChoice == "Sofa") {
                        Product newSofa = factories["Sofa"].CreateProduct(autoGeneratedArticle, nameChoice, priceChoice, supplierId, "Sofa");

                        AddingProduct(newSofa, supplierChoice, discountChoice, warrantyChoice);
                    }
                    else {
                        Product newFurniture = factories["Other furniture"].CreateProduct(autoGeneratedArticle, nameChoice, priceChoice, supplierId);

                        AddingProduct(newFurniture, supplierChoice, discountChoice, warrantyChoice);
                    }
                    break;
                default:
                    DefaultErrorOutput("Factory for this category is not implemented yet.");
                    break;
            }

            return BackToMenuPrompt();
        }

        public string Delete() => DeleteObjectScreen<Product, string>(
            _appData.Products,
            _appData.DeleteProductByArticle,
            product => $"[red]\"{product.Name}\"[/] ({product.Article})"
        );

        public string Edit() => this.EditObjectScreen(_appData.Products);

        public string Search() {
            ShowHeader("Search product");
            var searchChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose search option:")
                    .AddChoices(
                        "Fast filter",
                        "Thorough search"
                    )
            );

            double maxPrice = PromptWithDefaultValue("Enter the maximum price (in dollars):", 10000.0);

            double minPrice = PromptWithDefaultValue("Enter the minimum price (in dollars):", 0);

            List<Product> foundProducts;

            if (searchChoice == "Fast filter") {
                foundProducts = _appData.Products.FindProductsInPriceRange(minPrice, maxPrice);
            }
            else {
                string query = DefaultTextPrompt<string>("Enter search text (name or category):");
                foundProducts = ParallelSearchService.FastSearch(_appData.Products, query, maxPrice, minPrice);
            }

            ShowHeader("Search product");
            if (Tools.ValidateArray(foundProducts)) {
                DefaultErrorOutput("No products found for your search.");
            }
            else {
                RenderProductsTable(foundProducts);
            }

            return BackToMenuPrompt();
        }

        public string FilterByCategory() {
            ShowHeader("Filter by catergory");

            List<string> categoriesList;

            try {
                categoriesList = _appData.Products.GetCategoriesList().ToList();
            }
            catch (NullReferenceException ex) {
                DefaultErrorOutput(ex.Message);
                return BackToMenuPrompt();
            }

            var selectedCategory = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose category:")
                    .AddChoices(categoriesList)
                    .UseConverter(c => c.AddSpacesBetweenCapitalizedWords())
            );

            var filteredProducts = _appData.Products.FilterByCategory(selectedCategory);

            RenderProductsTable(filteredProducts);
            return BackToMenuPrompt();
        }
    }
}
