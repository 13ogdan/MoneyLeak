using System;

namespace Entity
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool Income { get; set; }
        public string Details { get; set; }

        public int? CategoryId{ get;set; }
        public Category Category{ get;set; }
    }
}
