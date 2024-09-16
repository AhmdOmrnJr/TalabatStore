using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpecifications(ProductSpecsParams specsParams)
            : base(p =>
                        (string.IsNullOrEmpty(specsParams.Search) || p.Name.Contains(specsParams.Search)) &&
                        (!specsParams.BrandId.HasValue || p.BrandId == specsParams.BrandId.Value) &&
                        (!specsParams.CategoryId.HasValue || p.CategoryId == specsParams.CategoryId.Value)
                  )
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);

            if (!string.IsNullOrEmpty(specsParams.Sort))
            {
                switch (specsParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;

                    case "priceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else
                AddOrderBy(p => p.Name);

            ApplyPagination((specsParams.PageIndex - 1) * specsParams.PageSize, specsParams.PageSize);
        }

        public ProductWithBrandAndCategorySpecifications(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }
    }
}
