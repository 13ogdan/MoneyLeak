using System;
using System.Collections.Generic;
using System.Text;
using EFStorage;
using Entity;

namespace ApplicationTest
{
    public class TestDBContentBuilder
    {
        private readonly AccountingDBContext _context;

        public TestDBContentBuilder(AccountingDBContext context)
        {
            _context = context;
        }

        public PaymentBuilder Payment()
        {
            var paymentBuilder = new PaymentBuilder();
            paymentBuilder.OnBuild += payment =>
            {
                _context.Add(payment);
                _context.SaveChanges();
            };
            return paymentBuilder;
        }
    }

    //TODO use builder where possible
    public class PaymentBuilder
    {
        private readonly Payment _payment;
        public event Action<Payment> OnBuild;

        public PaymentBuilder()
        {
            _payment = new Payment();
        }
        
        public PaymentBuilder WithDetails(string details)
        {
            _payment.Details = details;
            return this;
        }

        public PaymentBuilder WithDate(DateTime dateTime)
        {
            _payment.Date = dateTime;
            return this;
        }

        public PaymentBuilder WithCategory(Category category)
        {
            _payment.Category = category;
            return this;
        }

        public PaymentBuilder WithAmount(decimal amount)
        {
            _payment.Amount = amount;
            return this;
        }

        public Payment Add()
        {
            OnBuild?.Invoke(_payment);
            return _payment;
        }

        
    }
}
