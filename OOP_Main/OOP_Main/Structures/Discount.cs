using System;

namespace OOP_Main.Structures {
    public readonly struct Discount {
        public double Percent { get; }

        public Discount(double discountPercent) {
            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(
                    message: $"Discount percent should range between 0% and 100%", 
                    paramName: nameof(discountPercent)
                );

            Percent = discountPercent;
        }
    }
}
