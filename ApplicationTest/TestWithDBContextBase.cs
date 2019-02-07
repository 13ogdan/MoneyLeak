using EFStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTest
{
    [TestClass]
    public class TestWithDBContextBase
    {
        protected AccountingDBContext _context;
        private TestDBContentBuilder _dbContentBuilder;

        [TestInitialize]
        public void BaseTestInitialize()
        {
            var options = new DbContextOptionsBuilder<AccountingDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;

            _context = new AccountingDBContext(options);

            _dbContentBuilder = new TestDBContentBuilder(_context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        protected TestDBContentBuilder DbContentBuilder => _dbContentBuilder;
    }
}
