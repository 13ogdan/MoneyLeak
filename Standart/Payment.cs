using System;

namespace BankReport
{
    internal class Payment : IPayment
    {
        public DateTime Date { get;internal set; }
        public double Amount { get; internal set; }
        public string Details { get; internal set; }
    }
}
