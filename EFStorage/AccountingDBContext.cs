using System;
using EFStorage.Configuration;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace EFStorage
{
    public class AccountingDBContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Details> Details { get; set; }
        //public DbSet<PaymentCategory> PaymentCategories { get; set; }

        public AccountingDBContext()
        {

        }

        public AccountingDBContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DetailsConfiguration());
        }
    }
}
