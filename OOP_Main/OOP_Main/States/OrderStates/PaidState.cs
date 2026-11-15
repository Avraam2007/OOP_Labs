using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States.OrderStates {
    public class PaidState : OrderState {
        public override string GetDescription() => "Paid, ready to ship";
        public override bool CanBeCancelled() => true;

        public override string Pay() {
            return "The order is already paid.";
        }

        public override string Ship() {
            return "The order will be shipped later";
        }

        public override string Cancel() {
            _order.Status = OrderStatus.Cancelled;
            _order.SetState(new CancelledState());
            return "The order is cancelled.";
        }
    }
}
