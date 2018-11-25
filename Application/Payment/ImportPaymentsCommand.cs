using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using MediatR;

namespace Application.Payment
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

        public Task<Unit> Handle(ImportPaymentsCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
