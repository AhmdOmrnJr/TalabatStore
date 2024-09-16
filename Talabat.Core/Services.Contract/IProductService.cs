using Talabat.Core.Entities;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.Core.Services.Contract
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product?>> GetProductsAsync(ProductSpecsParams specsParams);
        Task<Product?> GetProductAsync(int productId);
        Task<int> GetCountAsync(ProductSpecsParams specsParams);
        Task<IReadOnlyList<ProductBrand>> GetBrandsAsync();
        Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync();
    }
}
