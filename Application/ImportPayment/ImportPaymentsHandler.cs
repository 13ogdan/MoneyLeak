// <copyright>BSP corporation</copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using Entity;
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
            foreach (var paymentInfo in importResult.ValidPayments)
            {
                if (paymentInfo.PaymentId == null)
                    throw new InvalidOperationException("Payment has no generated id. Parsing report has been done with problems.");

                //TODO we need to improve details saving
                var details = await _dBContext.Details.FindAsync(paymentInfo.Details) ?? new Details() { FullDetails = paymentInfo.Details };

                if (paymentInfo.Income)
                    await CreateOrUpdateIncome(cancellationToken, paymentInfo, details);
                else
                    await CreateOrUpdatePayment(cancellationToken, paymentInfo, details);

            }

            var savedPayments = await _dBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //TODO send notification that we have new payments
            //TODO send notification about problematic report lines
            return Unit.Value;
        }

        private async Task CreateOrUpdatePayment(CancellationToken cancellationToken, PaymentInfo paymentInfo, Details details)
        {
            var payment = new Payment()
            {
                Id = paymentInfo.PaymentId,
                Amount = paymentInfo.Amount,
                Date = paymentInfo.Date,
                Details = details,
                Category = details.DefaultCategory,
            };

            if (await _dBContext.Payments.FindAsync(payment.Id) == null)
                await _dBContext.Payments.AddAsync(payment, cancellationToken).ConfigureAwait(false);
        }

        private async Task CreateOrUpdateIncome(CancellationToken cancellationToken, PaymentInfo paymentInfo, Details details)
        {
            var income = new Income()
            {
                Id = paymentInfo.PaymentId,
                Amount = paymentInfo.Amount,
                Date = paymentInfo.Date,
                Details = details,
            };

            if (await _dBContext.Incomes.FindAsync(income.Id) == null)
                await _dBContext.Incomes.AddAsync(income, cancellationToken).ConfigureAwait(false);
        }
    }


}