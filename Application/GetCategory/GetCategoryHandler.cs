using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EFStorage;
using Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.GetCategory
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryRequest, IEnumerable<Category>>
    {
        private readonly AccountingDBContext _dbContext;

        public GetCategoryHandler(AccountingDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Category>> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Categories.Where(category => category.Name.StartsWith(request.Keyword, true, CultureInfo.InvariantCulture));
            return await query.ToArrayAsync(cancellationToken: cancellationToken);
        }
    }

    public class GetCategoryRequest : IRequest<IEnumerable<Category>>
    {
        public string Keyword { get; set; }
    }
}
