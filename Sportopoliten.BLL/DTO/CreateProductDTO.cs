using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO
{
    public class CreateProductDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public List<ProductVariantDTO> Variants { get; set; }
    }
}
