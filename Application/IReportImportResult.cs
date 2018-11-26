using Entity;
using System.Collections.Generic;

namespace Application
{
    public interface IReportImportResult
    {
        ICollection<string> InvalidPaymentsLine { get; }
        ICollection<Payment> ValidPayments { get; }
    }
}