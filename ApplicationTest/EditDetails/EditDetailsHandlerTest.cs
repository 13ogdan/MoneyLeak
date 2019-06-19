// <copyright>BSP corporation</copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.EditDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTest.EditDetails
{
    [TestClass]
    public class EditDetailsHandlerTest : TestWithDBContextBase
    {
        private EditDetailsHandler _editDetailsHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _editDetailsHandler = new EditDetailsHandler(_context);
        }

        [TestMethod]
        public async Task Should_AddDetailsAlias()
        {
            var details = DbContentBuilder.Details().WithDetails("Details").Build();
            details.Alias = "NewAlias";

            await _editDetailsHandler.Handle(new EditDetailsRequest
            {
                Details = details
            }, CancellationToken.None);

            var savedDetails = await _context.Details.FindAsync(details.FullDetails);
            Assert.AreEqual("NewAlias", savedDetails.Alias);
        }

        [TestMethod]
        public async Task Should_ChangeDetailsAlias()
        {
            var details = DbContentBuilder.Details().WithDetails("Details").WithAlias("Alias").Build();
            details.Alias = "NewAlias";

            await _editDetailsHandler.Handle(new EditDetailsRequest
            {
                Details = details
            }, CancellationToken.None);

            var savedDetails = await _context.Details.FindAsync(details.FullDetails);
            Assert.AreEqual("NewAlias", savedDetails.Alias);
        }

        [TestMethod]
        public async Task Should_AddDetailsCategory()
        {
            var details = DbContentBuilder.Details().WithDetails("Details").WithAlias("Alias").Build();
            details.DefaultCategory = null;

            await _editDetailsHandler.Handle(new EditDetailsRequest
            {
                Details = details,
                NewDefaultCategoryName = "New category"
            }, CancellationToken.None);

            var savedDetails = await _context.Details.FindAsync(details.FullDetails);
            Assert.IsNotNull(savedDetails.DefaultCategory);
            Assert.AreEqual("New category", savedDetails.DefaultCategory.Name);
        }

        [TestMethod]
        public async Task Should_ChangeDetailsCategory_To_NewOne()
        {
            var category = DbContentBuilder.Category().CreateWithRandomData();
            var details = DbContentBuilder.Details().WithDetails("Details").WithAlias("Alias").WithCategory(category).Build();

            await _editDetailsHandler.Handle(new EditDetailsRequest
            {
                Details = details,
                NewDefaultCategoryName = "New category"
            }, CancellationToken.None);

            var savedDetails = await _context.Details.FindAsync(details.FullDetails);
            Assert.IsNotNull(savedDetails.DefaultCategory);
            Assert.AreEqual("New category", savedDetails.DefaultCategory.Name);
        }

        [TestMethod]
        public async Task Should_ChangeDetailsCategory_To_ExistingOne()
        {
            var category = DbContentBuilder.Category().CreateWithRandomData();
            var newCategory = DbContentBuilder.Category().CreateWithRandomData();
            var details = DbContentBuilder.Details().WithDetails("Details").WithAlias("Alias").WithCategory(category).Build();

            await _editDetailsHandler.Handle(new EditDetailsRequest
            {
                Details = details,
                NewDefaultCategoryName = newCategory.Name.ToLower()
            }, CancellationToken.None);

            var savedDetails = await _context.Details.FindAsync(details.FullDetails);
            Assert.IsNotNull(savedDetails.DefaultCategory);
            Assert.AreEqual(newCategory.Name, savedDetails.DefaultCategory.Name);
        }

        [TestMethod]
        public async Task Should_ApplyCategoryToAllPayments_With_SameDetailsAndNoCategory()
        {
            var details = DbContentBuilder.Details().WithDetails("Details").WithAlias("Alias").Build();
            var newCategory = DbContentBuilder.Category().CreateWithRandomData();
            //Add payments to check that is taught only needed
            for (int i = 0; i < 10; i++)
                DbContentBuilder.Payment().WithRandomData().Build();

            //Add payments with the same details but already set category
            for (int i = 0; i < 10; i++)
                DbContentBuilder.Payment().WithRandomData().WithDetails(details).WithCategory(new CategoryBuilder().CreateWithRandomData()).Build();

            var paymentsToChange = new List<string>();
            for (int i = 0; i < 10; i++)
                paymentsToChange.Add(DbContentBuilder.Payment().WithRandomData().WithCategory(null).WithDetails(details).Build().PaymentId);

            await _editDetailsHandler.Handle(new EditDetailsRequest
            {
                Details = details,
                NewDefaultCategoryName = newCategory.Name.ToLower()
            }, CancellationToken.None);

            var savedDetails = await _context.Payments.Where(payment => payment.Category == newCategory).Select(payment => payment.PaymentId).ToArrayAsync();
            Assert.AreEqual(10, savedDetails.Length);
            CollectionAssert.AreEquivalent(paymentsToChange, savedDetails);
        }
    }
}