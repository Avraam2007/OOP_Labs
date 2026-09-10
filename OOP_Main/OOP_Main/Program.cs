using System;
using System.Collections.Generic;

namespace OOP_Main {
    internal class Program {
        static void Main(string[] args) {
            Supplier firstSupplier = new Supplier(1234, "Good Inc.", "example@mail.com", 4.5);
            Supplier secondSupplier = new Supplier(123, "bad Inc.", "dexample@mail.com", 1.5);

            EnginePiston piston = new EnginePiston("148", "SuperPiston", 5.697, 30, "Aluminium", 32.2, firstSupplier.SupplierId);

            ElectricMotor newPart = new ElectricMotor("149", "SuperMotor", 10.001, 1500, 250, 110.8, 0.75, firstSupplier.SupplierId);

            LightBulb bulb = new LightBulb("150", "SuperLightBulb", 4, 1500, 250, 2500, 2000, secondSupplier.SupplierId);
            Wire wire = new Wire("151", "SuperWire", 6.69, 0.5, "Aluminium", 0.5, 2000, secondSupplier.SupplierId);

            firstSupplier.AddPartToCatalog(piston);
            firstSupplier.AddPartToCatalog(bulb);

            secondSupplier.AddPartToCatalog(newPart);
            secondSupplier.AddPartToCatalog(wire);

            firstSupplier.ShowInfo();
            secondSupplier.ShowInfo();

            List<Part> listToOrder = new List<Part> {
                firstSupplier.GetProductFromCatalogByName(piston.Name),
                secondSupplier.GetProductFromCatalogByName(newPart.Name),
                secondSupplier.GetProductFromCatalogByName(wire.Name),
            };

            Order newOrder = new Order(123, listToOrder);
            newOrder.AddPart(firstSupplier.GetProductFromCatalogByArticle(bulb.Article));
            newOrder.PrintOrder();

            Console.WriteLine("\n\nPress Enter to exit...\n");
            Console.ReadLine();
        }
    }
}
