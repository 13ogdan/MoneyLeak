// <copyright>BSP corporation</copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.GetPayment
{
    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, IEnumerable<Payment>>
    {
        private readonly AccountingDBContext _accountingDbContext;

        public GetPaymentsQueryHandler(AccountingDBContext accountingDbContext)
        {
            _accountingDbContext = accountingDbContext;
        }

        public async Task<IEnumerable<Payment>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Payment> payments = _accountingDbContext.Payments;
            payments = FilterByDate(payments, request);
            payments = FilterByPhrase(payments, request);
            payments = FilterByCategory(payments, request);

            return await payments.ToArrayAsync(cancellationToken);
        }

        private IQueryable<Payment> FilterByCategory(IQueryable<Payment> payments, GetPaymentsQuery request)
        {
            if (!request.WithEmptyCategory)
                return payments;

            return payments.Where(payment => payment.Category == null);
        }

        private IQueryable<Payment> FilterByPhrase(IQueryable<Payment> payments, GetPaymentsQuery request)
        {
            var ignoreDetails = string.IsNullOrWhiteSpace(request.WithPhraseInDetails);
            if (ignoreDetails)
                return payments;

            return payments.Where(payment => IsDetailsValid(payment, request.WithPhraseInDetails));
        }

        //TODO convert to lambda
        bool IsDetailsValid(Payment payment, string keyword)
        {
            var detailsAlias = payment.Details?.Alias + payment.Details?.FullDetails;

            if (string.IsNullOrEmpty(detailsAlias))
                return false;

            var result = detailsAlias.Contains(keyword, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        private IQueryable<Payment> FilterByDate(IQueryable<Payment> payments, GetPaymentsQuery request)
        {
            if (request.DateTo == null && request.DateFrom == null)
                return payments;

            return payments.Where(payment =>
                (request.DateFrom == null || payment.Date >= request.DateFrom) &&
                (request.DateTo == null || payment.Date <= request.DateTo));
        }
    }
}