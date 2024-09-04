using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpecifications(string? sort, int? brandId, int? categoryId)
            : base(p => 
                        (!brandId.HasValue || brandId == p.BrandId) &&
                        (!categoryId.HasValue || categoryId == p.CategoryId))
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case ("priceAsc"):
                        AddOrderBy(p => p.Price);
                        break;

                    case ("priceDesc"):
                        AddOrderByDesc(p => p.Price);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else
                AddOrderBy(p => p.Name);
        }

        public ProductWithBrandAndCategorySpecifications(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }
    }
}
