// <copyright>BSP corporation</copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.ImportPayment;
using Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApplicationTest.ImportPayment
{
    [TestClass]
    internal sealed class ImportPaymentsHandlerTest : TestWithDBContextBase
    {
        private ImportPaymentsHandler _importPayment;
        private readonly ICollection<Payment> _payments = new List<Payment>();

        [TestInitialize]
        public void TestInitialize()
        {
            var report = new Mock<IReportImportResult>();
            report.Setup(result => result.ValidPayments).Returns(() => _payments);

            var importReportHelper = new Mock<IImportReportHelper>();
            importReportHelper.Setup(helper => helper.Import(It.IsAny<string[]>())).Returns(() => report.Object);

            _importPayment = new ImportPaymentsHandler(_context, importReportHelper.Object);
        }

        [TestMethod]
        public async Task Should_AddNewEmptyPayment()
        {
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid().ToString()
            };
            _payments.Add(payment);

            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);

            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.IsNotNull(savedPayment);
        }

        [TestMethod]
        public async Task Should_AddNewPayment()
        {
            var payment = CreatePayment();
            _payments.Add(payment);

            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);

            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.IsNotNull(savedPayment);
            Assert.AreEqual(payment.Amount, savedPayment.Amount);
            Assert.AreEqual(payment.Date, savedPayment.Date);
            Assert.AreEqual(payment.Details, savedPayment.Details);
            Assert.IsNull(savedPayment.Category);
        }

        [TestMethod]
        public async Task Should_NotAddDuplicateOrChange_If_PaymentExistsAsync()
        {
            //Arrange
            var payment = CreatePayment();
            _payments.Add(payment);

            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);
            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            savedPayment.Category = new Category() { Name = "Category", CategoryId = 0 };
            await _context.SaveChangesAsync();
            //Act
            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);
            //Assert
            savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.IsNotNull(savedPayment.Category);
        }

        public Payment CreatePayment()
        {
            var paymentDate = DateTime.Today;
            var details = "Buy beer for Tka4";

            return new Payment
            {
                Amount = 100.0m,
                Date = paymentDate,
                Details = details,
                PaymentId = Guid.NewGuid().ToString()
            };
        }
    }
}