using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.FileIO;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class ErrorRegistryTests {
        [TestInitialize]
        public void Setup() {
            ErrorRegistry.RegisterHandler(null);
        }

        [TestCleanup]
        public void Cleanup() {
            ErrorRegistry.RegisterHandler(null);
        }

        [TestMethod]
        public void HandleError_NoHandlerRegistered_DoesNotThrow() {
            ErrorRegistry.HandleError("Unhandled error message");
        }

        [TestMethod]
        public void RegisterHandlerAndHandleError_ValidHandler_InvokesHandlerWithExactMessage() {
            string receivedMessage = null;
            int callCount = 0;

            ErrorRegistry.RegisterHandler(msg => {
                receivedMessage = msg;
                callCount++;
            });

            ErrorRegistry.HandleError("Disk full error");

            Assert.AreEqual(1, callCount);
            Assert.AreEqual("Disk full error", receivedMessage);
        }

        [TestMethod]
        public void RegisterHandler_CalledMultipleTimes_ReplacesPreviousHandler() {
            int firstHandlerCalls = 0;
            int secondHandlerCalls = 0;
            string secondReceived = null;

            ErrorRegistry.RegisterHandler(_ => firstHandlerCalls++);
            ErrorRegistry.RegisterHandler(msg => {
                secondHandlerCalls++;
                secondReceived = msg;
            });

            ErrorRegistry.HandleError("Second handler message");

            Assert.AreEqual(0, firstHandlerCalls);
            Assert.AreEqual(1, secondHandlerCalls);
            Assert.AreEqual("Second handler message", secondReceived);
        }

        [TestMethod]
        public void RegisterHandler_SetToNull_StopsInvokingPreviousHandler() {
            int callCount = 0;
            ErrorRegistry.RegisterHandler(_ => callCount++);

            ErrorRegistry.HandleError("First call");
            Assert.AreEqual(1, callCount);

            ErrorRegistry.RegisterHandler(null);
            ErrorRegistry.HandleError("Second call");

            Assert.AreEqual(1, callCount);
        }
    }
}
