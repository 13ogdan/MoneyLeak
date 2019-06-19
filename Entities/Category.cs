using System;

namespace Entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
    }
}
