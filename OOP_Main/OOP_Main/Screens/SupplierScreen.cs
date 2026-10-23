using OOP_Main.Comparers;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main.Screens {
    public class SupplierScreen: BaseScreen, IScreen {
        public SupplierScreen(Data appData): base(appData) { }

        public string Show() {
            ShowHeader("Suppliers");

            var sortOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose sorting option:")
                    .AddChoices("By ID (default)", "By rating (from the best)", "By name (A-Z)")
            );

            List<Supplier> suppliersToSort = new List<Supplier>(_appData.Suppliers);

            switch (sortOption) {
                case "By ID (default)":
                    suppliersToSort.Sort(new SupplierIDComparer());
                    break;
                case "By rating (from the best)":
                    suppliersToSort.Sort(new SupplierRatingComparer());
                    suppliersToSort.Reverse();
                    break;
                case "By name (A-Z)":
                    suppliersToSort.Sort(new SupplierNameComparer());
                    break;
            }
            return ShowListScreen("Suppliers", suppliersToSort);
        }

        public string Create() {
            ShowHeader("Creating supplier");

            string name = DefaultTextPrompt<string>("Enter supplier [green]name[/]:");

            string email = DefaultTextPrompt<string>("Enter supplier [green]e-mail[/]:");

            string phone = DefaultTextPrompt<string>("Enter supplier [green]phone number[/]:");

            double rating = DefaultTextPrompt<double>("Enter supplier [green]rating (from 1.0 to 5.0)[/]:");

            try {
                _appData.AddSupplier(name, email, phone, rating);
                DefaultSuccessOutput($"New supplier is created! You can check it on \"{GetEnumDescription(MenuOption.ShowSuppliers)}\" screen.");
            }
            catch (ArgumentException ex) {
                DefaultErrorOutput(ex.Message);
                return BackToMenuPrompt();
            }

            return BackToMenuPrompt();
        }

        public string Delete() => DeleteObjectScreen<Supplier, int>(
            _appData.Suppliers,
            _appData.DeleteSupplierById,
            supplier => $"[red]\"{supplier.Name}\"[/] ({supplier.SupplierId})"
        );

        public string Edit() => this.EditObjectScreen(_appData.Suppliers);
    }
}
