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
            var payments = _accountingDbContext.Payments.Where(payment => !payment.Income && FilterByDate(payment, request)
                                                                && FilterByPhrase(payment, request) && FilterByCategory(payment, request));
            
            return await payments.ToArrayAsync(cancellationToken);
        }

        private bool FilterByCategory(Payment payment, GetPaymentsQuery request)
        {
            return !request.WithEmptyCategory || (request.WithEmptyCategory && payment.Category == null);
        }

        private bool FilterByPhrase(Payment payment, GetPaymentsQuery request)
        {
            return string.IsNullOrWhiteSpace(request.WithPhraseInDetails) || payment.Details.Contains(request.WithPhraseInDetails);
        }

        private bool FilterByDate(Payment payment, GetPaymentsQuery request)
        {
            return request.Date == null || payment.Date == request.Date;
        }
    }
}