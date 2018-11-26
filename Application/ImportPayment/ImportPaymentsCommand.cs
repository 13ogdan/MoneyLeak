using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using MediatR;

namespace Application.ImportPayment
{
    public class ImportPaymentsCommand : IRequest
    {
        public ImportPaymentsCommand(string[] report)
        {
            Report = report;
        }

        public string[] Report { get; }
    }

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
                //TODO check if already exists
                await _dBContext.Payments.AddAsync(payment).ConfigureAwait(false);
                
            }

            return Unit.Value;
        }
    }
}
