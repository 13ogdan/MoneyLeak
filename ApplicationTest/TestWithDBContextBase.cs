using EFStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTest
{
    [TestClass]
    public class TestWithDBContextBase
    {
        protected AccountingDBContext _context;

        [TestInitialize]
        public void BaseTestInitialize()
        {
            var options = new DbContextOptionsBuilder<AccountingDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;

            _context = new AccountingDBContext(options);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }
    }
}
