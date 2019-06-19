// <copyright>BSP corporation</copyright>

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using Entity;
using MediatR;

namespace Application.EditDetails
{
    public class EditDetailsHandler : IRequestHandler<EditDetailsRequest>
    {
        private readonly AccountingDBContext _accountingDbContext;

        public EditDetailsHandler(AccountingDBContext accountingDbContext)
        {
            _accountingDbContext = accountingDbContext;
        }

        public async Task<Unit> Handle(EditDetailsRequest request, CancellationToken cancellationToken)
        {
            var requestDetails = request.Details;
            var categoryChanged = !string.Equals(request.NewDefaultCategoryName, requestDetails.DefaultCategory?.Name,
                                                 StringComparison.InvariantCultureIgnoreCase);

            if (categoryChanged)
            {
                var category = _accountingDbContext.Categories.FirstOrDefault(
                        c => c.Name.Equals(request.NewDefaultCategoryName, StringComparison.InvariantCultureIgnoreCase));

                if (category == null)
                    category = await CreateNewCategoryWithName(request, cancellationToken);

                requestDetails.DefaultCategory = category;

                UpdateCategoryByDefaultCategoryInDetails(requestDetails);
            }

            _accountingDbContext.Details.Update(requestDetails);
            await _accountingDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        private async Task<Category> CreateNewCategoryWithName(EditDetailsRequest request, CancellationToken cancellationToken)
        {
            var categoryAddResult = await _accountingDbContext.Categories.AddAsync(new Category()
            {
                Name = request.NewDefaultCategoryName
            }, cancellationToken);
            var category = categoryAddResult.Entity;
            return category;
        }

        private void UpdateCategoryByDefaultCategoryInDetails(Details details)
        {
            foreach (var payment in _accountingDbContext.Payments.Where(payment => payment.Details == details && payment.Category == null))
            {
                payment.Category = details.DefaultCategory;
                _accountingDbContext.Payments.Update(payment);
            }
        }
    }

    public class EditDetailsRequest : IRequest
    {
        public Details Details { get; set; }

        public string NewDefaultCategoryName { get; set; }

        public Category NewDefaultCategory { get; set; }
    }
}