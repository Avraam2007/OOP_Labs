using OOP_Main.Entities;
using OOP_Main.Utilities;

namespace OOP_Main.Handlers {
    public class EmptyOrderValidationHandler : BaseOrderValidationHandler {
        public override bool Handle(Order order, User buyer, out string errorMessage) {
            if (order == null || !Tools.ValidateArray(order.Products)) {
                errorMessage = "Замовлення не містить жодного товару.";
                return false;
            }
            return base.Handle(order, buyer, out errorMessage);
        }
    }

    public class UserAccountStatusHandler : BaseOrderValidationHandler {
        public override bool Handle(Order order, User buyer, out string errorMessage) {
            if (buyer == null || !buyer.CanBuy()) {
                errorMessage = "Користувач не авторизований або не має прав для здійснення покупок.";
                return false;
            }
            return base.Handle(order, buyer, out errorMessage);
        }
    }

    public class StockAvailabilityHandler : BaseOrderValidationHandler {
        public override bool Handle(Order order, User buyer, out string errorMessage) {
            foreach (var product in order.Products) {
                if (string.IsNullOrWhiteSpace(product.Article) || !Tools.IsValidArticle(product.Article)) {
                    errorMessage = $"Товар '{product.Name}' має некоректний або відсутній артикул ({product.Article}).";
                    return false;
                }
            }
            return base.Handle(order, buyer, out errorMessage);
        }
    }

    public class InsufficientFundsHandler : BaseOrderValidationHandler {
        public override bool Handle(Order order, User buyer, out string errorMessage) {
            double requiredTotal = order.GetTotalPrice();
            if (buyer.Account.Sum < requiredTotal) {
                errorMessage = $"Not enough money! Required: {requiredTotal}$, balance: {buyer.Account.Sum}$.";
                return false;
            }
            return base.Handle(order, buyer, out errorMessage);
        }
    }

    public class OrderAmountLimitHandler : BaseOrderValidationHandler {
        private readonly double _maxOrderLimit;

        public OrderAmountLimitHandler(double maxOrderLimit = 5000.0) {
            _maxOrderLimit = maxOrderLimit;
        }

        public override bool Handle(Order order, User buyer, out string errorMessage) {
            if (order.GetTotalPrice() > _maxOrderLimit) {
                errorMessage = $"Сума замовлення перевищує максимально дозволений ліміт однієї транзакції ({_maxOrderLimit}$).";
                return false;
            }
            return base.Handle(order, buyer, out errorMessage);
        }
    }
}
