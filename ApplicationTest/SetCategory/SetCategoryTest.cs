// <copyright>BSP corporation</copyright>

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.SetCategory;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTest.SetCategory
{
    [TestClass]
    public class SetCategoryTest : TestWithDBContextBase
    {
        private string _duplicatedDetails;
        private Payment _payment;
        private SetCategoryHandler _setCategoryHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _setCategoryHandler = new SetCategoryHandler(_context);
            AddBasePayments();
            _payment = _context.Payments.First(payment => payment.Details.FullDetails == _duplicatedDetails);

        }

        [TestMethod]
        public async Task Should_SetCategory()
        {
            //Act
            var category = new Category
            {
                Name = "NewCategory"
            };
            var setCategoryCommand = new SetCategoryCommand
            {
                Payment = _payment,
                Category = category
            };

            //Act
            await _setCategoryHandler.Handle(setCategoryCommand, CancellationToken.None);

            //Assert
            var paymentInDB = _context.Payments.Find(_payment.Id);
            Assert.IsNotNull(paymentInDB.Category);
            Assert.AreEqual(category.Name, paymentInDB.Category.Name);

            var categoryInDB = _context.Categories.FirstOrDefault(c => category.Name == c.Name);
            Assert.IsNotNull(categoryInDB);
        }

        [TestMethod]
        public async Task Should_ChangeCategory()
        {
            //Act
            _payment.Category = new Category
            {
                Name = "OldCategory"
            };
            _context.Update(_payment);
            _context.SaveChanges();

            var category = new Category
            {
                Name = "NewCategory"
            };
            var setCategoryCommand = new SetCategoryCommand
            {
                Payment = _payment,
                Category = category
            };

            //Act
            await _setCategoryHandler.Handle(setCategoryCommand, CancellationToken.None);

            //Assert
            var paymentInDB = _context.Payments.Find(_payment.Id);
            Assert.IsNotNull(paymentInDB.Category);
            Assert.AreEqual(category.Name, paymentInDB.Category.Name);

            var categoryInDB = _context.Categories.FirstOrDefault(c => category.Name == c.Name);
            Assert.IsNotNull(categoryInDB);
        }

        [TestMethod]
        public async Task Should_SetCategoryToAllPaymentsWithTheSameDetails()
        {
            var newCategoryName = "NewCategory";
            var category = new Category
            {
                Name = newCategoryName
            };
            var setCategoryCommand = new SetCategoryCommand
            {
                Payment = _payment,
                Category = category,
                ApplyToAllWithSuchDetails = true
            };

            //Act
            await _setCategoryHandler.Handle(setCategoryCommand, CancellationToken.None);

            //Assert
            var samePaymentDetailsCount = _context.Payments.Count(payment => payment.Details == _payment.Details);
            var countOfAddedCategories = _context.Payments.Count(payment => newCategoryName == payment.Category.Name);

            Assert.AreEqual(samePaymentDetailsCount, countOfAddedCategories);
            Assert.AreEqual(2, countOfAddedCategories);
        }

        private void AddBasePayments()
        {
            _duplicatedDetails = "1";
            _context.Payments.Add(new PaymentBuilder().WithRandomData().WithDetails(_duplicatedDetails).Build());
            _context.Payments.Add(new PaymentBuilder().WithRandomData().WithDetails("2").Build());
            _context.Payments.Add(new PaymentBuilder().WithRandomData().WithDetails("3").Build());
            _context.Payments.Add(new PaymentBuilder().WithRandomData().WithDetails(_duplicatedDetails + 1).Build());
            _context.Payments.Add(new PaymentBuilder().WithRandomData().WithDetails(_duplicatedDetails + _duplicatedDetails).Build());
            _context.Payments.Add(new PaymentBuilder().WithRandomData().WithDetails(_duplicatedDetails).Build());
            _context.SaveChanges();
        }
    }
}