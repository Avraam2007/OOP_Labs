namespace OOP_Main.States.OrderStates {
    public class PendingState: OrderState {
        public override string GetDescription() => "Pending";
        public override bool CanBeCancelled() => true;

        public override string Pay() {
            _order.Status = OrderStatus.Paid;
            _order.SetState(new PaidState());
            return "The order is paid. Shipping...";
        }

        public override string Ship() {
            return "Error: we can't ship unpaid order.";
        }

        public override string Cancel() {
            _order.Status = OrderStatus.Cancelled;
            _order.SetState(new CancelledState());
            return "The order is cancelled.";
        }
    }
}
