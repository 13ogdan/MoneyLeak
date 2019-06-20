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
            var payments = _accountingDbContext.Payments.Where(payment =>
                                                                   FilterByDate(payment, request) &&
                                                                   FilterByPhrase(payment, request) &&
                                                                   FilterByCategory(payment, request));

            return await payments.ToArrayAsync(cancellationToken);
        }

        private bool FilterByCategory(Payment payment, GetPaymentsQuery request)
        {
            var result = !request.WithEmptyCategory || (request.WithEmptyCategory && payment.Category == null);
            return result;
        }

        private bool FilterByPhrase(Payment payment, GetPaymentsQuery request)
        {
            var ignoreDetails = string.IsNullOrWhiteSpace(request.WithPhraseInDetails);
            if (ignoreDetails)
                return true;

            var detailsAlias = payment.Details?.Alias + payment.Details?.FullDetails;
            if (string.IsNullOrEmpty(detailsAlias))
                return false;

            var result = detailsAlias.Contains(request.WithPhraseInDetails, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        private bool FilterByDate(Payment payment, GetPaymentsQuery request)
        {
            var result = (request.DateFrom == null || payment.Date >= request.DateFrom) &&
                (request.DateTo == null || payment.Date <= request.DateTo);
            return result;
        }
    }
}