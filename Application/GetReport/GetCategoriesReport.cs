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

namespace Application.GetReport
{
    public class GetCategoriesReport : IRequest<ICategoryReport>
    {
        public Category ParentCategory { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public interface ICategoryReport
    {
        IReadOnlyDictionary<Category, decimal> CategoriesOutcome { get; }
    }

    public class CategoryReport : ICategoryReport
    {
        private readonly Dictionary<Category, decimal> _categoriesOutcome;

        public CategoryReport()
        {
            _categoriesOutcome = new Dictionary<Category, decimal>();
        }

        public IReadOnlyDictionary<Category, decimal> CategoriesOutcome => _categoriesOutcome;

        public void Add(Category category, decimal sum)
        {
            _categoriesOutcome.Add(category, sum);
        }
    }

    public class GetCategoriesReportHandler : IRequestHandler<GetCategoriesReport, ICategoryReport>
    {
        private readonly AccountingDBContext _accountingDbContext;

        public GetCategoriesReportHandler(AccountingDBContext accountingDbContext)
        {
            _accountingDbContext = accountingDbContext;
        }

        public async Task<ICategoryReport> Handle(GetCategoriesReport request, CancellationToken cancellationToken)
        {
            var report = await _accountingDbContext.Payments
                .Where(payment => !payment.Income && payment.Category != null && payment.Category.ParentCategory == request.ParentCategory
                        && payment.Date >= request.StartDate && payment.Date <= request.EndDate).GroupBy(
                    payment => payment.Category, payment => payment.Amount, (category, outcomes) => new
                    {
                        category,
                        Sum = outcomes.Sum()
                    }).ToArrayAsync(cancellationToken);

            var categoryReport = new CategoryReport();
            foreach (var reportItem in report)
                categoryReport.Add(reportItem.category, reportItem.Sum);

            return categoryReport;
        }
    }
}