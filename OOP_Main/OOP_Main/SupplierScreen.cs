using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public string DeleteSupplierScreen() {
            AnsiConsole.Clear();
            ShowHeader("[bold]Delete supplier[/]");

            if (Tools.ValidateArray(_appData.Suppliers)) {
                AnsiConsole.MarkupLine("[red bold]There are no suppliers to delete.[/]");
                return BackToMenuPrompt();
            }

            var choices = new List<string>();
            foreach (var supplier in _appData.Suppliers) {
                choices.Add($"({supplier.SupplierId}) {supplier.Name}");
            }
            choices.Add("Cancel");

            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a supplier to delete:")
                    .PageSize(10)
                    .AddChoices(choices));

            if (selectedChoice == "Cancel") return BackToMenuPrompt();

            int id = Convert.ToInt32(selectedChoice.Split(')')[0].TrimStart('('));
            Supplier supplierToDelete = _appData.Suppliers.FirstOrDefault(sup => sup.SupplierId == id);

            if (supplierToDelete != null) {
                bool confirm = AnsiConsole.Confirm(
                    $"Are you sure you want to delete [red]\"{supplierToDelete.Name}\"[/] ({supplierToDelete.SupplierId})?",
                    defaultValue: false
                );

                if (confirm) {
                    bool isDeleted = _appData.DeleteSupplierByName(supplierToDelete.Name);
                    if (isDeleted) {
                        AnsiConsole.MarkupLine("[green]Supplier successfully deleted![/]");
                    }
                    else {
                        AnsiConsole.MarkupLine("[red]Failed to delete supplier.[/]");
                    }
                }
                else {
                    AnsiConsole.MarkupLine("[yellow]Deleting cancelled.[/]");
                }
            }

            return BackToMenuPrompt();
        }
    }
}
