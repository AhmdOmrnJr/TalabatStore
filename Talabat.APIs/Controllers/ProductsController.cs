using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : BaseController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _brandsRepo;
        private readonly IGenericRepository<ProductCategory> _categoryRepo;
        private readonly IMapper _mapper;

        public ProductsController(
            IGenericRepository<Product> ProductsRepo,
            IGenericRepository<ProductBrand> BrandsRepo,
            IGenericRepository<ProductCategory> CategoryRepo,
            IMapper mapper)
        {
            _productsRepo = ProductsRepo;
            _brandsRepo = BrandsRepo;
            _categoryRepo = CategoryRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(string? sort, int? brandId, int? categoryId)
        {
            var specs = new ProductWithBrandAndCategorySpecifications(sort, brandId, categoryId);
            var products = await _productsRepo.GetAllWithSpecsAsync(specs);
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
        }

        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var specs = new ProductWithBrandAndCategorySpecifications(id);
            var product = await _productsRepo.GetWithSpecsAsync(specs);

            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _brandsRepo.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var Categories = await _categoryRepo.GetAllAsync();
            return Ok(Categories);
        }
    }
}
