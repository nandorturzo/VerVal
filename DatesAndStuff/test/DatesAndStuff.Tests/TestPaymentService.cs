using System;

namespace DatesAndStuff.Tests
{
    internal class TestPaymentService : IPaymentService
    {
        private uint startCallCount = 0;
        private uint specifyCallCount = 0;
        private uint confirmCallCount = 0;
        private double balance;

        public TestPaymentService(double initialBalance)
        {
            balance = initialBalance;
        }

        public double Balance => balance;

        public void StartPayment()
        {
            if (startCallCount != 0 || specifyCallCount > 0 || confirmCallCount > 0)
                throw new Exception("StartPayment should only be called once, before any other action.");

            startCallCount++;
        }

        public void SpecifyAmount(double amount)
        {
            if (startCallCount != 1 || specifyCallCount > 0 || confirmCallCount > 0)
                throw new Exception("SpecifyAmount should only be called after StartPayment and before ConfirmPayment.");

            if (amount > balance)
            {
                throw new Exception("Insufficient balance for payment.");
            }

            specifyCallCount++;
        }

        public void ConfirmPayment()
        {
            if (startCallCount != 1 || specifyCallCount != 1 || confirmCallCount > 0)
                throw new Exception("ConfirmPayment should only be called once after SpecifyAmount.");

            confirmCallCount++;
        }

        public bool SuccessFul()
        {
            return startCallCount == 1 && specifyCallCount == 1 && confirmCallCount == 1;
        }

        public void Cancel()
        {
            startCallCount = 0;
            specifyCallCount = 0;
            confirmCallCount = 0;
        }
    }
}
