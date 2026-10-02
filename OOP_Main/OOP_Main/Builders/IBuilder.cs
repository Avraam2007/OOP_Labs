using System.Collections.Generic;

namespace OOP_Main.Builders {
    public interface IBuilder {
        void Reset();

        void SetOrderId(int orderId);

        void SetCustomer(int buyerId);

        void AddCatalog(List<Product> products);

        void SetStatus(OrderStatus status);
    }
}
