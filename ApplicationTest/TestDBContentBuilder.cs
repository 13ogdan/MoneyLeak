using System;
using System.Collections.Generic;
using System.Text;
using EFStorage;
using Entity;

namespace ApplicationTest
{
    //TODO move classes to separate assembly
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

    public class PaymentBuilder : EntityBuilder<Payment, PaymentBuilder>
    {
        public PaymentBuilder() : base(new Payment()) { }

        public PaymentBuilder WithDetails(string details)
        {
            _entity.Details = details;
            return this;
        }

        public PaymentBuilder WithDate(DateTime dateTime)
        {
            _entity.Date = dateTime;
            return this;
        }

        public PaymentBuilder WithCategory(Category category)
        {
            _entity.Category = category;
            return this;
        }

        public PaymentBuilder WithAmount(decimal amount)
        {
            _entity.Amount = amount;
            return this;
        }

        protected override void FillByRandomData()
        {
            _entity.Details = GetRandomString();
            var entityAmount = _random.NextDouble() * _random.Next(1, 10000);
            _entity.Amount = (decimal)entityAmount;
            _entity.Details = Guid.NewGuid().ToString();
            _entity.Category = new CategoryBuilder().CreateWithRandomData();
            _entity.Income = GetRandomBool();
            _entity.Date = GetRandomDateTime();
            _entity.PaymentId = GetRandomString();
        }

        protected static string GetRandomString()
        {
            return Guid.NewGuid().ToString();
        }

        protected bool GetRandomBool()
        {
            return _random.Next(0, 1) == 0;
        }

        protected DateTime GetRandomDateTime()
        {
            var randomDayBefore = DateTime.Now;
            randomDayBefore = randomDayBefore.Subtract(TimeSpan.FromDays(_random.Next(0, 365)));
            randomDayBefore = randomDayBefore.Subtract(TimeSpan.FromHours(_random.Next(0, 24)));
            randomDayBefore = randomDayBefore.Subtract(TimeSpan.FromSeconds(_random.Next(0, 60)));
            randomDayBefore = randomDayBefore.Subtract(TimeSpan.FromMinutes(_random.Next(0, 60)));
            return randomDayBefore;
        }
    }

    public abstract class EntityBuilder<T, TChildType> where TChildType : class
    {
        protected readonly T _entity;
        protected Random _random = new Random(DateTime.Now.Millisecond);

        public event Action<T> OnBuild;

        protected EntityBuilder(T entity)
        {
            _entity = entity;
        }

        public T Build()
        {
            OnBuild?.Invoke(_entity);
            return _entity;
        }

        public TChildType WithRandomData()
        {
            FillByRandomData();
            OnBuild?.Invoke(_entity);
            return this as TChildType;
        }

        public T CreateWithRandomData()
        {
            FillByRandomData();
            OnBuild?.Invoke(_entity);
            return _entity;
        }

        protected abstract void FillByRandomData();
    }

    public class CategoryBuilder : EntityBuilder<Category, CategoryBuilder>
    {
        public CategoryBuilder() : base(new Category()) { }

        public CategoryBuilder WithName(string name)
        {
            _entity.Name = name;
            return this;
        }

        protected override void FillByRandomData()
        {
            _entity.Name = Guid.NewGuid().ToString();
        }
    }
}