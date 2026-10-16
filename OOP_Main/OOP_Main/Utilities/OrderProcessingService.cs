using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OOP_Main.Utilities {
    public class OrderProcessingQueue : IDisposable {
        private readonly BlockingCollection<Order> _orderQueue = new BlockingCollection<Order>(boundedCapacity: 100);
        private readonly Task _processingTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public OrderProcessingQueue() {
            _processingTask = Task.Run(() => ProcessQueue(_cts.Token));
        }

        // Producer
        public void EnqueueOrder(Order order) {
            order.Status = OrderStatus.Paid;
            _orderQueue.Add(order);
            Logger.Log($"[Queue] Order #{order.OrderId} has been added to the processing queue", Logger.LogLevels.Info);
        }


        // Consumer
        private void ProcessQueue(CancellationToken ct) {
            foreach (var order in _orderQueue.GetConsumingEnumerable(ct)) {
                try {
                    //Thread.Sleep(300000);
                    Thread.Sleep(15000);

                    order.Status = OrderStatus.Shipped;
                    Logger.Log($"[Queue] Order #{order.OrderId} has been shipped from the store's warehouse.", Logger.LogLevels.Info);
                }
                catch (Exception ex) {
                    order.Status = OrderStatus.Cancelled;
                    Logger.Log($"[Queue] Error processing order #{order.OrderId}: {ex.Message}", Logger.LogLevels.Error);
                }
            }
        }

        public void Dispose() {
            _orderQueue.CompleteAdding();
            _cts.Cancel();
        }
    }
}
