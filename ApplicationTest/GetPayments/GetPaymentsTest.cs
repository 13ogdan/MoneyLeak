using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.GetPayment;
using Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTest.GetPayments
{
    [TestClass]
    public sealed class GetPaymentsTest : TestWithDBContextBase
    {
        private GetPaymentsQueryHandler _handler;
        private int _count = 5;

        [TestInitialize]
        public void TestInitialize()
        {
            _handler = new GetPaymentsQueryHandler(_context);
            CreateDefaultPayments();
        }

        [TestMethod]
        public async Task Should_ReturnAllPayments()
        {
            var result = await _handler.Handle(new GetPaymentsQuery(), CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(_count, payments.Length);
        }

        private void CreateDefaultPayments()
        {
            for (int i = 0; i < _count; i++)
                DbContentBuilder.Payment().Add();
        }

        //TODO add a possibility to set range
        [TestMethod]
        public async Task Should_ReturnPayments_For_SpecificDay()
        {
            var date = new DateTime(2018, 2, 18);
            DbContentBuilder.Payment().WithDate(date).Add();
            DbContentBuilder.Payment().WithDate(date).Add();
            var getPaymentsQuery = new GetPaymentsQuery() { Date = date };
            
            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(2, payments.Length);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_With_SpecificDetails()
        {
            DbContentBuilder.Payment().WithDetails("Buy beer in dopio").Add();
            DbContentBuilder.Payment().WithDetails("Buy coffee in dopio").Add();
            DbContentBuilder.Payment().WithDetails("Dopio lunch").Add();
            var getPaymentsQuery = new GetPaymentsQuery() { WithPhraseInDetails = "dopio"};
            
            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(3, payments.Length);
        }

        [TestMethod]
        public async Task Should_ReturnPayments_Without_Category()
        {
            DbContentBuilder.Payment().WithCategory(new Category()).Add();
            DbContentBuilder.Payment().WithCategory(new Category()).Add();
            var getPaymentsQuery = new GetPaymentsQuery() { WithEmptyCategory = true};
            
            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(_count, payments.Length);
        }


        [TestMethod]
        public async Task Should_ReturnPayments_By_CombinedFilter()
        {
            var date = new DateTime(2018, 2, 18);
            var details = "details";
            //One with everything but with category
            DbContentBuilder.Payment().WithDate(date).WithDetails(details).WithCategory(new Category()).Add();
            //One excellent
            DbContentBuilder.Payment().WithDate(date).WithDetails(details).Add();
            //Only date wrong
            DbContentBuilder.Payment().WithDate(date.AddDays(1)).WithDetails(details).Add();
            //Only details wrong
            DbContentBuilder.Payment().WithDate(date).WithDetails(details.Substring(1)).Add();

            DbContentBuilder.Payment().WithCategory(new Category()).Add();
            var getPaymentsQuery = new GetPaymentsQuery()
            {
                WithEmptyCategory = true,
                Date = date,
                WithPhraseInDetails = details
            };
            
            var result = await _handler.Handle(getPaymentsQuery, CancellationToken.None);
            var payments = result.ToArray();

            Assert.AreEqual(1, payments.Length);
        }
    }
}
