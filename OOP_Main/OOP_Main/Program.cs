using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    internal class Program {
        static void Main(string[] args) {
            User admin = new User("123", "Admin", "admin", true);
            Data appData = new Data();
            appData.AddUser(admin);

            Supplier firstSupplier = new Supplier(1234, "Good Inc.", "example@mail.com", 4.5);
            Supplier secondSupplier = new Supplier(123, "bad Inc.", "dexample@mail.com", 1.5);

            ElectronicProduct phone = new ElectronicProduct("148", "SuperPhone", 10.79, 65, 20, firstSupplier.SupplierId);

            Cloth tshirt = new Cloth("149", "SuperTShirt", 10.001, "Cotton", firstSupplier.SupplierId);

            Furniture chair = new Furniture("150", "SuperChair", 5, 2.5, 30, 30, 50, "Wood", secondSupplier.SupplierId);
            Sofa sofa = new Sofa("150", "SuperSofa", 10, 25, 300, 50, 50, "Wood", false, secondSupplier.SupplierId);

            firstSupplier.AddPartToCatalog(phone);
            firstSupplier.AddPartToCatalog(tshirt);

            secondSupplier.AddPartToCatalog(chair);
            secondSupplier.AddPartToCatalog(sofa);

            appData.AddSupplier(firstSupplier);
            appData.AddSupplier(secondSupplier);

            appData.AddProduct(phone);
            appData.AddProduct(tshirt);

            appData.AddProduct(chair);
            appData.AddProduct(sofa);

            List<Product> listToOrder = new List<Product> {
                firstSupplier.GetProductFromCatalogByName(phone.Name),
                secondSupplier.GetProductFromCatalogByName(chair.Name),
                secondSupplier.GetProductFromCatalogByName(sofa.Name),
            };

            Order newOrder = new Order(123, listToOrder);
            appData.AddOrder(newOrder);
            Interface consoleInterface = new Interface(appData);
            try {
                consoleInterface.BootUpScreen();
            }
            catch (Exception ex) {

                AnsiConsole.WriteException(ex);
            }
        }
    }
}
