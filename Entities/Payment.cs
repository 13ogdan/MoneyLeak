using System;

namespace Entity
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncomming { get; set; }
        public string Details { get; set; }
    }
}
