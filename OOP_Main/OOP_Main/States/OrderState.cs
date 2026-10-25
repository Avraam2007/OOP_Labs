namespace OOP_Main.States {
    public abstract class OrderState {
        protected Order _order;

        public void SetOrder(Order order) {
            this._order = order;
        }
        public abstract string GetDescription();
        public abstract bool CanBeCancelled();

        public abstract string Pay();
        public abstract string Ship();
        public abstract string Cancel();
    }
}
