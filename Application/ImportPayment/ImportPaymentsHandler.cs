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
            //TODO check what happen if entity exists.
            await _dBContext.Payments.AddRangeAsync(importResult.ValidPayments, cancellationToken).ConfigureAwait(false);
            var savedPayments = await _dBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //TODO send notification that we have new payments
            //TODO send notification about problematic report lines
            return Unit.Value;
        }
    }
}