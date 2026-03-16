using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int ProductsCount { get; set; }
    }
}