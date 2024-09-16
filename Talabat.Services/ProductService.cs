using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<Product?>> GetProductsAsync(ProductSpecsParams specsParams)
        {
            var specs = new ProductWithBrandAndCategorySpecifications(specsParams);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecsAsync(specs);
            return products;
        }
        public async Task<Product?> GetProductAsync(int productId)
        {
            var specs = new ProductWithBrandAndCategorySpecifications(productId);
            var product = await _unitOfWork.Repository<Product>().GetWithSpecsAsync(specs);
            return product;
        }

        public async Task<int> GetCountAsync(ProductSpecsParams specsParams)
        {
            var countSpec = new ProductWithFilterationForCountSpecifications(specsParams);
            var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            return count;
        }

        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
            => await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
            => await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
    }
}
