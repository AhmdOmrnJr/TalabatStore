using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecs
{
    public class ProductWithFilterationForCountSpecifications : BaseSpecifications<Product>
    {
        public ProductWithFilterationForCountSpecifications(ProductSpecsParams specsParams)
            : base(p =>
                        (string.IsNullOrEmpty(specsParams.Search) || p.Name.Contains(specsParams.Search)) &&
                        (!specsParams.BrandId.HasValue || p.BrandId == specsParams.BrandId.Value) &&
                        (!specsParams.CategoryId.HasValue || p.CategoryId == specsParams.CategoryId.Value)
                  )
        {
            
        }
    }
}
