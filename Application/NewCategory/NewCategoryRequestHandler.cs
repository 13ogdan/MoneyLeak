using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using Entity;
using MediatR;

namespace Application.NewCategory {
    public class NewCategoryRequestHandler : IRequestHandler<NewCategoryRequest, Category>
    {
        private readonly AccountingDBContext _accountingDbContext;

        public NewCategoryRequestHandler(AccountingDBContext accountingDbContext)
        {
            _accountingDbContext = accountingDbContext;
        }

        public async Task<Category> Handle(NewCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = _accountingDbContext.Add(new Category()
            {
                ParentCategory = request.ParentCategory,
                Name = request.Name
            });

            var id = await _accountingDbContext.SaveChangesAsync(cancellationToken);

            return category.Entity;
        }
    }
}