using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States.OrderStates {
    internal class PendingState: OrderState {
        public override string GetDescription() => "Pending";
        public override bool CanBeCancelled() => true;

        public override string Pay() {
            _order.Status = OrderStatus.Paid;
            return "The order is paid. Shipping...";
        }

        public override string Ship() {
            return "Error: we can't ship unpaid order.";
        }

        public override string Cancel() {
            _order.Status = OrderStatus.Cancelled;
            return "The order is cancelled.";
        }
    }
}
