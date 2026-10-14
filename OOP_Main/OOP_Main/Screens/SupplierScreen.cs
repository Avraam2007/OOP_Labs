using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main.Screens {
    public class SupplierScreen: BaseScreen, IScreen {
        public SupplierScreen(Data appData): base(appData) { }

        public string Show() => ShowListScreen("Suppliers", _appData.Suppliers);

        public string Create() {
            ClearConsole();
            ShowHeader("Creating supplier");

            string name = DefaultTextPrompt<string>("Enter supplier [green]name[/]:");

            string email = DefaultTextPrompt<string>("Enter supplier [green]e-mail[/]:");

            double rating = DefaultTextPrompt<double>("Enter supplier [green]rating (from 1.0 to 5.0)[/]:");

            _appData.AddSupplier(name, email, rating);
            DefaultSuccessOutput($"New supplier is created! You can check it on \"{GetEnumDescription(MenuOption.ShowSuppliers)}\" screen.");

            return BackToMenuPrompt();
        }

        public string Delete() => DeleteObjectScreen<Supplier, int>(
            _appData.Suppliers,
            _appData.DeleteSupplierById,
            supplier => $"[red]\"{supplier.Name}\"[/] ({supplier.SupplierId})"
        );
    }
}
