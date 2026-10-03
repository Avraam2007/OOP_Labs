using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States {
    public abstract class OrderState {
        protected Order _order;

        public void SetUser(Order order) {
            this._order = order;
        }
        public abstract string GetDescription();
        public abstract bool CanBeCancelled();

        public abstract string Pay();
        public abstract string Ship();
        public abstract string Cancel();
    }
}
