// <copyright>BSP corporation</copyright>

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.GetPayment;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTest.GetPayments
{
    [TestClass]
    public sealed class GetPaymentsTest : TestWithDBContextBase
    {
        private readonly int _count = 10;
        private GetPaymentsQueryHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            CreateDefaultPayments();
            _handler = new GetPaymentsQueryHandler(_context);
        }

        [TestMethod]
        public async Task Should_ReturnAllPayments()
        {
            var result = await _handler.Handle(new GetPaymentsQuery(), CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(_count, payments.Length);
        }

        //TODO Investigate why it happens
        [TestMethod, Ignore]
        public async Task MagicTest()
        {
            //var payments = _context.Payments.Where(payment => payment.Category == null).ToArray();
            var payments = _context.Payments.Include(payment => payment.Category).Where(payment => IsCategoryNull(payment)).ToArray();
            var expectedPayments = _context.Payments.Where(payment => payment.CategoryId == null).ToArray();

            Assert.AreEqual(expectedPayments.Length, payments.Length);
            CollectionAssert.AreEquivalent(expectedPayments, payments);
        }

        private static bool IsCategoryNull(Payment payment)
        {
            return payment.Category == null;
            //return payment.CategoryId == null;
        }

        [TestMethod]
        public async Task Should_ReturnPayments_For_SpecificDay()
        {
            var date = new DateTime(2018, 2, 18);
            DbContentBuilder.Payment().WithDate(date.AddDays(1)).Build();
            DbContentBuilder.Payment().WithDate(date.AddDays(-1)).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                DateFrom = date,
                DateTo = date
            };

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(2, payments.Length);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_After_SpecificDay()
        {
            var date = new DateTime(2018, 2, 18);
            DbContentBuilder.Payment().WithDate(date.AddDays(1)).Build();
            DbContentBuilder.Payment().WithDate(date.AddDays(-1)).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                DateFrom = date,
            };

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();


            var paymentsAfter = _context.Payments.Where(payment => payment.Date >= date).ToArray();
            Assert.AreEqual(paymentsAfter.Length , payments.Length);
            CollectionAssert.AreEquivalent(paymentsAfter, payments);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_Before_SpecificDay()
        {
            var date = new DateTime(2018, 2, 18);
            DbContentBuilder.Payment().WithDate(date.AddDays(1)).Build();
            DbContentBuilder.Payment().WithDate(date.AddDays(-1)).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                DateTo = date,
            };

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            var paymentsBefore = _context.Payments.Where(payment => payment.Date <= date).ToArray();
            Assert.AreEqual(paymentsBefore.Length , payments.Length);
            CollectionAssert.AreEquivalent(paymentsBefore, payments);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_InRangeOfDates()
        {
            var date = new DateTime(2018, 2, 18);
            DbContentBuilder.Payment().WithDate(date.AddDays(-2)).Build();
            DbContentBuilder.Payment().WithDate(date.AddDays(+2)).Build();
            DbContentBuilder.Payment().WithDate(date.AddDays(-1)).Build();
            DbContentBuilder.Payment().WithDate(date.AddDays(+1)).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            DbContentBuilder.Payment().WithDate(date).Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                DateTo = date.AddDays(1),
                DateFrom = date.AddDays(-1),
            };

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(4, payments.Length);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_With_SpecificDetails()
        {
            DbContentBuilder.Payment().WithDetails("Buy beer in dopio").Build();
            DbContentBuilder.Payment().WithDetails("Buy coffee in dopio").Build();
            DbContentBuilder.Payment().WithDetails("Dopio lunch").Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                WithPhraseInDetails = "dopio"
            };

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(3, payments.Length);
        }


        [TestMethod]
        public async Task Should_ReturnPayments_With_SpecificDetailsAlias()
        {
            var d1 = DbContentBuilder.Details().WithDetails("1").WithAlias("Buy beer in dopio").Build();
            var d2 = DbContentBuilder.Details().WithDetails("2").WithAlias("Buy coffee in dopio").Build();
            var d3 = DbContentBuilder.Details().WithDetails("3").WithAlias("Buy coffee in MC").Build();
            DbContentBuilder.Payment().WithDetails(d1).Build();
            DbContentBuilder.Payment().WithDetails(d2).Build();
            DbContentBuilder.Payment().WithDetails(d3).Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                WithPhraseInDetails = "dopio"
            };

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(2, payments.Length);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_Without_Category()
        {
            DbContentBuilder.Payment().WithCategory(null).Build();
            DbContentBuilder.Payment().WithCategory(null).Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                WithEmptyCategory = true
            };
            var ct = _context.ChangeTracker;

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            var paymentsWithoutCategory = _context.Payments.Where(payment => payment.Category == null).ToArray();
            Assert.AreEqual(paymentsWithoutCategory.Length , payments.Length);
            CollectionAssert.AreEquivalent(paymentsWithoutCategory, payments);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_By_CombinedFilter()
        {
            var date = new DateTime(2018, 2, 18);
            var details = "details";
            //One with everything but with category
            DbContentBuilder.Payment().WithDate(date).WithDetails(details).WithCategory(new Category()).Build();
            //One excellent
            DbContentBuilder.Payment().WithDate(date).WithDetails(details).Build();
            //Only date wrong
            DbContentBuilder.Payment().WithDate(date.AddDays(1)).WithDetails(details).Build();
            //Only details wrong
            DbContentBuilder.Payment().WithDate(date).WithDetails(details.Substring(1)).Build();

            DbContentBuilder.Payment().WithCategory(new Category()).Build();
            var getPaymentsQuery = new GetPaymentsQuery
            {
                WithEmptyCategory = true,
                DateFrom = date,
                DateTo = date,
                WithPhraseInDetails = details
            };

            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(1, payments.Length);
        }

        private void CreateDefaultPayments()
        {
            var paymentBuilder = DbContentBuilder.Payment();
            for (int i = 0; i < _count; i++)
            {
                paymentBuilder.WithRandomData().Build();
            }
        }
    }
}