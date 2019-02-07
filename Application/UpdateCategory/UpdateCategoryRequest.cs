// <copyright>BSP corporation</copyright>

using Entity;
using MediatR;

namespace Application.UpdateCategory
{
    public class UpdateCategoryRequest : IRequest
    {
        private Category Category { get; set; }
    }
}