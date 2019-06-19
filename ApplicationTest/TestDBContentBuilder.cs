// <copyright>BSP corporation</copyright>

using System;
using Application;
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
            return CreateBuilder<PaymentBuilder,Payment>();
        }

        public CategoryBuilder Category()
        {
            return CreateBuilder<CategoryBuilder,Category>();
        }

        public DetailsBuilder Details()
        {
            return CreateBuilder<DetailsBuilder,Details>();
        }

        private T CreateBuilder<T,TEntity>() where T: EntityBuilder<TEntity,T>, new()
        {
            var builder = new T();
            builder.OnBuild += entity =>
            {
                _context.Add(entity);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };
            return builder;
        }
    }

    public abstract class EntityBuilder<T, TChildType> where TChildType : class
    {
        protected readonly T _entity;
        protected readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public event Action<T> OnBuild;

        protected EntityBuilder(T entity)
        {
            _entity = entity;
        }

        public TChildType WithRandomData()
        {
            FillByRandomData();
            return this as TChildType;
        }

        public T Build()
        {
            OnBuild?.Invoke(_entity);
            return _entity;
        }

        public T CreateWithRandomData()
        {
            FillByRandomData();
            OnBuild?.Invoke(_entity);
            return _entity;
        }

        protected abstract void FillByRandomData();

        protected static string GetRandomString()
        {
            var randomString = Guid.NewGuid().ToString();
            return randomString;
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

    public class PaymentBuilder : EntityBuilder<Payment, PaymentBuilder>
    {
        public PaymentBuilder() : base(new Payment()) { }

        public PaymentBuilder WithDetails(string details)
        {
            _entity.Details = new Details()
            {
                FullDetails = details
            };
            return this;
        }

        public PaymentBuilder WithDetails(Details details)
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
            var entityAmount = _random.NextDouble() * _random.Next(1, 10000);
            _entity.Amount = (decimal)entityAmount;
            _entity.Details = new DetailsBuilder().CreateWithRandomData();

            _entity.Category = GetRandomBool() ? new CategoryBuilder().CreateWithRandomData() : _entity.Details.DefaultCategory;

            _entity.Income = GetRandomBool();
            _entity.Date = GetRandomDateTime();
            _entity.PaymentId = GetRandomString();
        }
    }


    public class PaymentInfoBuilder : EntityBuilder<PaymentInfo, PaymentInfoBuilder>
    {
        public PaymentInfoBuilder() : base(new PaymentInfo()) { }

        public PaymentInfoBuilder WithDetails(string details)
        {
            _entity.Details = details;
            return this;
        }

        public PaymentInfoBuilder WithDate(DateTime dateTime)
        {
            _entity.Date = dateTime;
            return this;
        }

        public PaymentInfoBuilder WithAmount(decimal amount)
        {
            _entity.Amount = amount;
            return this;
        }

        protected override void FillByRandomData()
        {
            var entityAmount = _random.NextDouble() * _random.Next(1, 10000);
            _entity.Amount = (decimal)entityAmount;
            _entity.Details = GetRandomString();
            _entity.Income = GetRandomBool();
            _entity.Date = GetRandomDateTime();
            _entity.PaymentId = GetRandomString();
        }
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

    public class DetailsBuilder : EntityBuilder<Details, DetailsBuilder>
    {
        public DetailsBuilder() : base(new Details()) { }

        public DetailsBuilder WithDetails(string details)
        {
            _entity.FullDetails = details;
            return this;
        }

        public DetailsBuilder WithAlias(string alias)
        {
            _entity.Alias = alias;
            return this;
        }

        public DetailsBuilder WithCategory(Category category)
        {
            _entity.DefaultCategory = category;
            return this;
        }

        protected override void FillByRandomData()
        {
            _entity.FullDetails = GetRandomString();
            if (GetRandomBool())
                _entity.Alias = GetRandomString();
            if (GetRandomBool())
                _entity.DefaultCategory = new CategoryBuilder().WithRandomData().Build();
        }
    }
}