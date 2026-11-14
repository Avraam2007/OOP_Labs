using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.States;
using OOP_Main.Utilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading;

namespace OOP_UnitTests.Utilities {
    [TestClass]
    public class OrderProcessingQueueTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private OrderProcessingQueue _queue;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_OrderQueueTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);
        }

        [TestCleanup]
        public void Cleanup() {
            _queue?.Dispose();
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                Directory.Delete(_tempWorkingDir, recursive: true);
            }
        }

        #region Producer (EnqueueOrder) & Dispose Tests

        [TestMethod]
        public void EnqueueOrder_ValidOrder_SetsStatusToPaidAndAddsToQueue() {
            _queue = new OrderProcessingQueue();
            var order = new Order(101, 5);
            Assert.AreEqual(OrderStatus.Pending, order.Status);

            _queue.EnqueueOrder(order);

            Assert.AreEqual(OrderStatus.Paid, order.Status);
        }

        [TestMethod]
        public void EnqueueOrder_AfterDispose_ThrowsInvalidOperationException() {
            _queue = new OrderProcessingQueue();
            _queue.Dispose();

            var order = new Order(102, 5);
            Assert.Throws<InvalidOperationException>(() => 
                _queue.EnqueueOrder(order));
        }

        #endregion

        #region Consumer (ProcessQueue) Tests

        [TestMethod]
        public void ProcessQueue_OrderConsumed_TransitionsStatusFromPaidToShipped() {
            _queue = new OrderProcessingQueue();
            var order = new Order(201, 10);

            _queue.EnqueueOrder(order);
            Assert.AreEqual(OrderStatus.Paid, order.Status);

            // У ProcessQueue встановлена затримка Thread.Sleep(15000), тому очікуємо перехід у Shipped з запасом часу
            bool isShipped = SpinWait.SpinUntil(() => order.Status == OrderStatus.Shipped, TimeSpan.FromSeconds(18));

            Assert.IsTrue(isShipped, "Замовлення не перейшло у статус Shipped після обробки в черзі.");
        }

        [TestMethod]
        public void ProcessQueue_NullOrderInCollection_CatchesExceptionWithoutCrashingConsumer() {
            _queue = new OrderProcessingQueue();

            // Отримуємо внутрішню колекцію _orderQueue, щоб додати null і спровокувати NullReferenceException у споживачі
            FieldInfo queueField = typeof(OrderProcessingQueue).GetField("_orderQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            var internalCollection = (BlockingCollection<Order>)queueField.GetValue(_queue);

            internalCollection.Add(null, TestContext.CancellationToken);

            var validOrder = new Order(202, 11);
            _queue.EnqueueOrder(validOrder);

            // Перевіряємо, що після винятку на null споживач не впав і продовжив обробку наступного замовлення
            bool isNextOrderShipped = SpinWait.SpinUntil(() => validOrder.Status == OrderStatus.Shipped, TimeSpan.FromSeconds(33));

            Assert.IsTrue(isNextOrderShipped, "Споживач черги мав перехопити виняток на null та обробити наступне замовлення.");
        }

        public TestContext TestContext { get; set; }

        #endregion
    }
}
