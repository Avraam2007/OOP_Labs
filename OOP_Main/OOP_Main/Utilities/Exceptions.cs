using System;

namespace OOP_Main.Utilities {
    public class Exceptions {
        public class ECommerceException : Exception {
            public DateTime Timestamp { get; } = DateTime.Now;

            public ECommerceException(string message) : base(message) { }
            public ECommerceException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class EntityNotFoundException : ECommerceException {
            public string EntityName { get; }
            public object EntityId { get; }

            public EntityNotFoundException(string entityName, object entityId)
                : base($"Entity '{entityName}' with ID [{entityId}] not found.") {
                EntityName = entityName;
                EntityId = entityId;
            }
        }

        public class InsufficientFundsException : ECommerceException {
            public double RequiredAmount { get; }
            public double CurrentBalance { get; }

            public InsufficientFundsException(double requiredAmount, double currentBalance)
                : base($"Not enough! Requires: {requiredAmount}$, balance: {currentBalance}$.") {
                RequiredAmount = requiredAmount;
                CurrentBalance = currentBalance;
            }
        }

        public class InvalidDomainDataException : ECommerceException {
            public string PropertyName { get; }

            public InvalidDomainDataException(string propertyName, string message)
                : base($"Validation error for the field '{propertyName}': {message}") {
                PropertyName = propertyName;
            }
        }
    }
}
