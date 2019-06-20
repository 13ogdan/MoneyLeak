using System;

namespace Entity
{
    public class Payment
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public Details Details { get; set; }

        public Category Category{ get;set; }
    }

    public class Income
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public Details Details { get; set; }
    }
}
