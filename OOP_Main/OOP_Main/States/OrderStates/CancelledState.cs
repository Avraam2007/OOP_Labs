using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States.OrderStates {
    public class CancelledState : OrderState {
        public override string GetDescription() => "Cancelled";
        public override bool CanBeCancelled() => false;

        public override string Pay() {
            return "The order is cancelled.";
        }

        public override string Ship() {
            return "The order is cancelled.";
        }

        public override string Cancel() {
            return "The order is cancelled.";
        }
    }
}
