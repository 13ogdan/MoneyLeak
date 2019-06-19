using System;
using Entity;
using System.Collections.Generic;

namespace Application
{
    public interface IReportImportResult
    {
        ICollection<string> InvalidPaymentsLine { get; }
        ICollection<PaymentInfo> ValidPayments { get; }
    }

    public class PaymentInfo
    {
        public string PaymentId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool Income { get; set; }
        public string Details { get; set; }
    }
}