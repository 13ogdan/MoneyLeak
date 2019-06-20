// <copyright>BSP corporation</copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.ImportPayment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApplicationTest.ImportPayment
{
    [TestClass]
    public sealed class ImportPaymentsHandlerTest : TestWithDBContextBase
    {
        private readonly ICollection<PaymentInfo> _payments = new List<PaymentInfo>();
        private ImportPaymentsHandler _importPayment;

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
            var payment = new PaymentInfoBuilder().WithRandomData().IsIncome(false).Build();
            _payments.Add(payment);

            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);

            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.IsNotNull(savedPayment);
        }

        [TestMethod]
        public async Task Should_AddNewPayment()
        {
            var payment = new PaymentInfoBuilder().WithRandomData().IsIncome(false).Build();
            _payments.Add(payment);

            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);

            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.IsNotNull(savedPayment);
            Assert.AreEqual(payment.Amount, savedPayment.Amount);
            Assert.AreEqual(payment.Date, savedPayment.Date);
            Assert.AreEqual(payment.Details, savedPayment.Details.FullDetails);
            Assert.IsNull(savedPayment.Category);
        }

        [TestMethod]
        public async Task Should_NotAddDuplicateOrChange_If_PaymentExistsAsync()
        {
            //Arrange
            var payment = new PaymentInfoBuilder().WithRandomData().IsIncome(false).Build();
            _payments.Add(payment);

            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);
            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            savedPayment.Category = new CategoryBuilder().CreateWithRandomData();
            await _context.SaveChangesAsync();

            //Act
            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);

            //Assert
            savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.IsNotNull(savedPayment.Category);
        }

        [TestMethod]
        public async Task Should_ApplyDefaultCategory()
        {
            //Arrange
            var details = DbContentBuilder.Details().CreateWithRandomData();
            details.DefaultCategory = DbContentBuilder.Category().CreateWithRandomData();

            var payment = new PaymentInfoBuilder().WithRandomData().IsIncome(false).Build();
            payment.Details = details.FullDetails;
            _payments.Add(payment);

            //Act
            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);

            //Assert
            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.AreEqual(savedPayment.Category, details.DefaultCategory);
            Assert.AreEqual(savedPayment.Details, details);
        }

        [TestMethod]
        public async Task Should_AddNewDetails()
        {
            //Arrange
            var details = DbContentBuilder.Details().CreateWithRandomData();
            details.DefaultCategory = DbContentBuilder.Category().CreateWithRandomData();

            var someDetails = "Some details";
            var payment = new PaymentInfoBuilder().WithRandomData().WithDetails(someDetails).IsIncome(false).Build();
            _payments.Add(payment);

            //Act
            await _importPayment.Handle(new ImportPaymentsCommand(null), CancellationToken.None);

            //Assert
            var savedPayment = await _context.Payments.FindAsync(payment.PaymentId);
            Assert.AreEqual(savedPayment.Details.FullDetails, someDetails);
            var savedDetails = await _context.Details.FindAsync(someDetails);
            Assert.IsNotNull(savedDetails);
        }

        //TODO write unit tests for Income table
    }
}