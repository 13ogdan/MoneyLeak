using System.Collections.Generic;
using Entity;

namespace Application
{
    public interface IReportImportResult
    {
        ICollection<string> InvalidPaymentsLine { get; }
        ICollection<Payment> ValidPayments { get; }
    }
}