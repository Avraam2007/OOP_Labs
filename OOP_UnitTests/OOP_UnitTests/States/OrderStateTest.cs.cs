using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.States;
using OOP_Main.States.OrderStates;

namespace OOP_UnitTests.States {
    [TestClass]
    public class OrderStateTest {
        private OrderState _state;
        private OOP_Main.Order _testOrder;

        [TestInitialize]
        public void Init() {
            _testOrder = new OOP_Main.Order(1, 2);
        }

        [TestCleanup]
        public void Cleanup() {
            _testOrder = null;
            _state = null;
        }


        #region ShippingState Tests

        [TestMethod]
        public void ShippingState_StatusTransitionsAndCancelling_CanNotPayAndNotCancel() {
            _state = new ShippingState();
            _state.SetOrder(_testOrder);

            Assert.IsFalse(_state.CanBeCancelled());
            Assert.AreEqual("Shipping", _state.GetDescription());
            Assert.AreEqual("The order is already paid.", _state.Pay());
            Assert.AreEqual("The order is on the road.", _state.Ship());
            Assert.AreEqual("Too late: the order is shipping, unable to cancel.", _state.Cancel());
        }

        #endregion


        #region PendingState Tests

        [TestMethod]
        public void PendingState_DescriptionAndShipping_CannotShipStatusIsPending() {
            _state = new PendingState();
            _state.SetOrder(_testOrder);

            Assert.AreEqual("Pending", _state.GetDescription());
            Assert.AreEqual("Error: we can't ship unpaid order.", _state.Ship());
        }

        [TestMethod]
        public void PendingState_Cancelling_CanCancel() {
            _state = new PendingState();
            _state.SetOrder(_testOrder);

            Assert.IsTrue(_state.CanBeCancelled());
            Assert.AreEqual("The order is cancelled.", _state.Cancel());

            _state.Cancel();

            Assert.AreEqual(OrderStatus.Cancelled, _testOrder.Status);
        }

        [TestMethod]
        public void PendingState_Paying_CanPay() {
            _state = new PendingState();
            _state.SetOrder(_testOrder);

            Assert.AreEqual("The order is paid. Shipping...", _state.Pay());

            _state.Pay();

            Assert.AreEqual(OrderStatus.Paid, _testOrder.Status);
        }

        #endregion

        #region PaidState Tests

        [TestMethod]
        public void PaidState_DescriptionAndShipping_CannotShipStatusIsPending() {
            _state = new PaidState();
            _state.SetOrder(_testOrder);

            Assert.AreEqual("Paid, ready to ship", _state.GetDescription());
            Assert.AreEqual("The order will be shipped later", _state.Ship());
        }

        [TestMethod]
        public void PaidState_Cancelling_CanCancel() {
            _state = new PaidState();
            _state.SetOrder(_testOrder);

            Assert.IsTrue(_state.CanBeCancelled());
            Assert.AreEqual("The order is cancelled.", _state.Cancel());

            _state.Cancel();

            Assert.AreEqual(OrderStatus.Cancelled, _testOrder.Status);
        }

        [TestMethod]
        public void PaidState_Paying_CannotPay() {
            _state = new PaidState();
            _state.SetOrder(_testOrder);

            Assert.AreEqual("The order is already paid.", _state.Pay());
        }

        #endregion

        #region CancelledState Tests

        [TestMethod]
        public void CancelledState_DescriptionAndShipping_CannotShipStatusIsPending() {
            _state = new CancelledState();
            _state.SetOrder(_testOrder);

            Assert.AreEqual("Cancelled", _state.GetDescription());
            Assert.AreEqual("The order is cancelled.", _state.Ship());
        }

        [TestMethod]
        public void CancelledState_Cancelling_CannotCancel() {
            _state = new CancelledState();
            _state.SetOrder(_testOrder);

            Assert.IsFalse(_state.CanBeCancelled());
            Assert.AreEqual("The order is cancelled.", _state.Cancel());
        }

        [TestMethod]
        public void CancelledState_Paying_CannotPay() {
            _state = new CancelledState();
            _state.SetOrder(_testOrder);

            Assert.AreEqual("The order is cancelled.", _state.Pay());
        }

        #endregion
    }
}
