using Spectre.Console;

namespace OOP_Main {
    public class SupplierScreen: UIHelper {
        private readonly Data _appData;
        public SupplierScreen(Data appData) {
            _appData = appData;
        }

        public string ShowSuppliersScreen() => ShowListScreen("Suppliers", _appData.Suppliers);

        public string AddSupplierScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Creating supplier[/]");

            int supplierId = DefaultTextPrompt<int>("Enter supplier [green]ID[/]:");

            string name = DefaultTextPrompt<string>("Enter supplier [green]name[/]:");

            string email = DefaultTextPrompt<string>("Enter supplier [green]e-mail[/]:");

            double rating = DefaultTextPrompt<double>("Enter supplier [green]rating (from 1.0 to 5.0)[/]:");

            Supplier newSupplier = new Supplier(supplierId, name, email, rating);

            _appData.AddSupplier(newSupplier);
            AnsiConsole.MarkupLine($"[green bold]New supplier is created! You can check it on \"{GetEnumDescription(MenuOption.ShowSuppliers)}\" screen.[/]");

            return BackToMenuPrompt();
        }
    }
}
