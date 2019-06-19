using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using MediatR;

namespace Application.ImportPayment
{
    public class ImportPaymentsHandler : IRequestHandler<ImportPaymentsCommand>
    {
        private readonly AccountingDBContext _dBContext;
        private readonly IImportReportHelper _importReport;

        public ImportPaymentsHandler(AccountingDBContext dBContext, IImportReportHelper importReport)
        {
            _dBContext = dBContext;
            _importReport = importReport;
        }

        public async Task<Unit> Handle(ImportPaymentsCommand request, CancellationToken cancellationToken)
        {
            var importResult = _importReport.Import(request.Report);
            foreach (var payment in importResult.ValidPayments)
            {
                if (payment.PaymentId == null)
                    throw new InvalidOperationException("Payment has no generated id. Parsing report has been done with problems.");

                if (await _dBContext.Payments.FindAsync(payment.PaymentId) == null)
                    await _dBContext.Payments.AddAsync(payment, cancellationToken).ConfigureAwait(false);
            }

            var savedPayments = await _dBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //TODO send notification that we have new payments
            //TODO send notification about problematic report lines
            return Unit.Value;
        }
    }
}