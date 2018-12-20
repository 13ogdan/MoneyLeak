using System;
using System.Collections.Generic;
using System.Text;
using Entity;
using MediatR;

namespace Application.NewCategory
{
    public class NewCategoryRequest : IRequest<Category>
    {
        public string Name { get; set; }
        public Category ParentCategory { get; set; }
    }
}
