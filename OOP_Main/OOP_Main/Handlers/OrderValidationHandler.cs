using OOP_Main.Entities;

namespace OOP_Main.Handlers {
    public interface IOrderValidationHandler {
        IOrderValidationHandler SetNext(IOrderValidationHandler handler);
        bool Handle(Order order, User buyer, out string errorMessage);
    }

    public abstract class BaseOrderValidationHandler : IOrderValidationHandler {
        private IOrderValidationHandler _nextHandler;

        public IOrderValidationHandler SetNext(IOrderValidationHandler handler) {
            _nextHandler = handler;
            return handler;
        }

        public virtual bool Handle(Order order, User buyer, out string errorMessage) {
            if (_nextHandler != null) {
                return _nextHandler.Handle(order, buyer, out errorMessage);
            }
            errorMessage = null;
            return true;
        }
    }
}
