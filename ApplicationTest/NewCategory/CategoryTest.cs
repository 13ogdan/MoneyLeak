// <copyright>BSP corporation</copyright>

using System.Threading;
using System.Threading.Tasks;
using Application.NewCategory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTest.NewCategory
{
    [TestClass]
    internal sealed class CategoryTest : TestWithDBContextBase
    {
        private NewCategoryRequestHandler _newCategoryHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _newCategoryHandler = new NewCategoryRequestHandler(_context);
        }

        [TestMethod]
        public async Task Should_AddCategory_With_Name()
        {
            var expected = "Name";
            var newCategoryRequest = new NewCategoryRequest
            {
                Name = expected
            };

            var category = await _newCategoryHandler.Handle(newCategoryRequest, CancellationToken.None);

            Assert.IsNotNull(category.CategoryId);
            Assert.IsNull(category.ParentCategory);
            Assert.AreEqual(expected, category.Name);

            category = await _context.Categories.FindAsync(category.CategoryId);
            Assert.IsNull(category.ParentCategory);
            Assert.AreEqual(expected, category.Name);
        }

        //TODO move to update category handler
        //[TestMethod]
        //public void Should_UpdateCategoryName()
        //{
        //    _context.Categories.AddAsync(new Category(){})
        //    var expected = "NewName";
        //    var newCategoryRequest = new NewCategoryRequest(){Name = expected};

        //    var category = await _newCategoryHandler.Handle(newCategoryRequest, CancellationToken.None);

        //    Assert.IsNotNull(category.CategoryId);
        //    Assert.IsNull(category.ParentCategory);
        //    Assert.AreEqual(expected, category.Name);
        //}

        //[TestMethod]
        //public void Should_UpdateCategoryParent()
        //{
        //    _context.Categories.AddAsync(new Category() { })
        //        var expected = "NewName";
        //    var newCategoryRequest = new NewCategoryRequest() { Name = expected };

        //    var category = await _newCategoryHandler.Handle(newCategoryRequest, CancellationToken.None);

        //    Assert.IsNotNull(category.CategoryId);
        //    Assert.IsNull(category.ParentCategory);
        //    Assert.AreEqual(expected, category.Name);
        //}

        [TestMethod]
        public void Should_DetectCycle()
        {
            //TODO
        }
    }
}