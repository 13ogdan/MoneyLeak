using System;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace EFStorage
{
    public class AccountingDBContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        //public DbSet<PaymentCategory> PaymentCategories { get; set; }

        public AccountingDBContext()
        {

        }

        public AccountingDBContext(DbContextOptions options) : base(options)
        {

        }
    }
}
