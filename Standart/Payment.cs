using System;

namespace BankReport
{
    internal class Payment : IPayment
    {
        public DateTime Date { get;internal set; }
        public double Amout { get; internal set; }
        public string Details { get; internal set; }
    }
}
