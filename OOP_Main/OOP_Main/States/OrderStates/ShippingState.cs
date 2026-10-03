using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States.OrderStates {
    internal class ShippingState: OrderState {
        public override string GetDescription() => "Shipping";
        public override bool CanBeCancelled() => false;

        public override string Pay() {
            return "The order is already paid.";
        }

        public override string Ship() {
            return "The order is on the road.";
        }

        public override string Cancel() {
            return "Too late: the order is shipping, unable to cancel.";
        }
    }
}
