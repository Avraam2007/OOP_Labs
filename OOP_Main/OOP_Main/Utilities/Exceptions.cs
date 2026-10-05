using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.Utilities {
    public class Exceptions {
        // 1. Базовий виняток системи (Base Exception)
        public class ECommerceException : Exception {
            public DateTime Timestamp { get; } = DateTime.Now;

            public ECommerceException(string message) : base(message) { }
            public ECommerceException(string message, Exception innerException) : base(message, innerException) { }
        }

        // 2. Похідний виняток: Сутність не знайдено (Unchecked / Domain exception)
        public class EntityNotFoundException : ECommerceException {
            public string EntityName { get; }
            public object EntityId { get; }

            public EntityNotFoundException(string entityName, object entityId)
                : base($"Entity '{entityName}' with ID [{entityId}] not found.") {
                EntityName = entityName;
                EntityId = entityId;
            }
        }

        // 3. Похідний виняток: Недостатньо коштів (Checked-equivalent / Business rule violation)
        public class InsufficientFundsException : ECommerceException {
            public double RequiredAmount { get; }
            public double CurrentBalance { get; }

            public InsufficientFundsException(double requiredAmount, double currentBalance)
                : base($"Not enough! Потрібно: {requiredAmount}$, доступно: {currentBalance}$.") {
                RequiredAmount = requiredAmount;
                CurrentBalance = currentBalance;
            }
        }

        // 4. Похідний виняток: Помилка некоректних даних (Validation exception)
        public class InvalidDomainDataException : ECommerceException {
            public string PropertyName { get; }

            public InvalidDomainDataException(string propertyName, string message)
                : base($"Помилка валідації для поля '{propertyName}': {message}") {
                PropertyName = propertyName;
            }
        }
    }
}
