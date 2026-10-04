using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class SupplierScreen: BaseScreen {
        public SupplierScreen(Data appData): base(appData) { }

        public override string Show() => ShowListScreen("Suppliers", _appData.Suppliers);

        public override string Create() {
            ClearConsole();
            ShowHeader("[bold]Creating supplier[/]");

            string name = DefaultTextPrompt<string>("Enter supplier [green]name[/]:");

            string email = DefaultTextPrompt<string>("Enter supplier [green]e-mail[/]:");

            double rating = DefaultTextPrompt<double>("Enter supplier [green]rating (from 1.0 to 5.0)[/]:");

            _appData.AddSupplier(name, email, rating);
            DefaultMarkupOutput($"[green bold]New supplier is created! You can check it on \"{GetEnumDescription(MenuOption.ShowSuppliers)}\" screen.[/]");

            return BackToMenuPrompt();
        }

        public override string Delete() {
            ClearConsole();
            ShowHeader("[bold]Delete supplier[/]");

            if (Tools.ValidateArray(_appData.Suppliers)) {
                DefaultMarkupOutput("[red bold]There are no suppliers to delete.[/]");
                return BackToMenuPrompt();
            }

            var choices = new List<string>();
            foreach (var supplier in _appData.Suppliers) {
                // $"({supplier.SupplierId}) {supplier.Name}"
                choices.Add(supplier.ToString());
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
                        DefaultMarkupOutput("[green]Supplier successfully deleted![/]");
                    }
                    else {
                        DefaultMarkupOutput("[red]Failed to delete supplier.[/]");
                    }
                }
                else {
                    DefaultMarkupOutput("[yellow]Deleting cancelled.[/]");
                }
            }

            return BackToMenuPrompt();
        }
    }
}
