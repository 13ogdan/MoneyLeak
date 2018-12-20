using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using EFStorage;
using Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.SetCategory
{
    public class SetCategoryCommand:IRequest
    {
        public Payment Payment { get; set; }
        public Category Category { get; set; }
        public bool ApplyToAllWithSuchDetails { get; set; }
    }

    public class SetCategoryHandler : IRequestHandler<SetCategoryCommand>
    {
        private AccountingDBContext _accountingDbContext;

        public SetCategoryHandler(AccountingDBContext accountingDbContext)
        {
            _accountingDbContext = accountingDbContext;
        }

        public async Task<Unit> Handle(SetCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.ApplyToAllWithSuchDetails)
            {
                var payments = await _accountingDbContext.Payments.Where(payment => payment.Details == request.Payment.Details).ToArrayAsync(cancellationToken: cancellationToken);
                foreach (var payment in payments)
                {
                    payment.Category = request.Category;
                }
            }
            else
            {
                request.Payment.Category = request.Category;
            }

            await _accountingDbContext.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}
