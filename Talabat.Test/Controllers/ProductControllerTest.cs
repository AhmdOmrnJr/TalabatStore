using Moq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Controllers;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.APIs.Helper;
using Talabat.Core.Specifications.ProductSpecs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly IMapper _mapper;
        private readonly ProductsController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup mock IConfiguration to return "http://localhost" for "BaseUrl"
            _mockConfiguration.Setup(config => config["BaseUrl"]).Returns("https://localhost:7295");

            // Configure services for dependency injection
            var services = new ServiceCollection();

            // Register the mocked IConfiguration
            services.AddSingleton(_mockConfiguration.Object);

            // Register the ProductPictureUrlResolver
            services.AddTransient<ProductPictureUrlResolver>();

            // Register AutoMapper with the actual MappingProfiles
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfiles>(); // Ensure this is your actual mapping profile
            }, typeof(MappingProfiles).Assembly);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get the IMapper instance
            _mapper = serviceProvider.GetRequiredService<IMapper>();

            _mapper.ConfigurationProvider.AssertConfigurationIsValid();

            // Instantiate the controller with mocked service and configured mapper
            _controller = new ProductsController(_mockProductService.Object, _mapper);
        }

        // Helper method to create test products
        private Product CreateTestProduct(int id, string name) => new Product
        {
            Id = id,
            Name = name,
            Description = "Test Description",
            Price = 100m,
            BrandId = 1,
            Brand = new ProductBrand { Id = 1, Name = "Brand A" },
            CategoryId = 1,
            Category = new ProductCategory { Id = 1, Name = "Category A" },
            PictureUrl = $"images/{name.ToLower()}.jpg"
        };

        [Fact]
        public async Task GetProducts_WithDefaultPagination_ReturnsOkResult_WithDefaultPageSizeAndIndex()
        {
            // Arrange
            var specsParams = new ProductSpecsParams(); // Defaults: PageSize = 5, PageIndex = 1

            var products = new List<Product>
            {

                CreateTestProduct(1, "ProductA")

                ///new Product
                ///{
                ///    Id = 1,
                ///    Name = "Product A",
                ///    Description = "Description A",
                ///    Price = 500m,
                ///    BrandId = 1,
                ///    Brand = new ProductBrand { Id = 1, Name = "Brand A" },
                ///    CategoryId = 1,
                ///    Category = new ProductCategory { Id = 1, Name = "Category A" },
                ///    PictureUrl = "images/productA.jpg"
                ///}
            };

            int totalCount = 1;

            _mockProductService.Setup(service => service.GetProductsAsync(specsParams))
                .ReturnsAsync(products);

            _mockProductService.Setup(service => service.GetCountAsync(specsParams))
                .ReturnsAsync(totalCount);

            // Act
            var result = await _controller.GetProducts(specsParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagination = Assert.IsType<Pagination<ProductToReturnDto>>(okResult.Value);

            // Verify pagination metadata
            Assert.Equal(5, pagination.PageSize); // Default PageSize
            Assert.Equal(1, pagination.PageIndex); // Default PageIndex
            Assert.Equal(totalCount, pagination.Count);

            // Verify data
            Assert.Single(pagination.Data);
            var returnedProduct = pagination.Data[0];
            Assert.Equal("ProductA", returnedProduct.Name);
            Assert.Equal("Test Description", returnedProduct.Description);
            Assert.Equal("https://localhost:7295/images/producta.jpg", returnedProduct.PictureUrl); // Resolved URL
            Assert.Equal(100m, returnedProduct.Price);
            Assert.Equal(1, returnedProduct.BrandId);
            Assert.Equal("Brand A", returnedProduct.Brand);
            Assert.Equal(1, returnedProduct.CategoryId);
            Assert.Equal("Category A", returnedProduct.Category);
        }

        [Fact]
        public async Task GetProducts_WithValidSpecsParams_ReturnsOkResult_WithPaginatedProducts()
        {
            // Arrange
            var specsParams = new ProductSpecsParams
            {
                PageSize = 2,
                PageIndex = 1,
                BrandId = 1,
                CategoryId = 1,
                Sort = "priceAsc",
                Search = "Laptop"
            };

            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Laptop A",
                    Description = "High-end laptop",
                    Price = 1500m,
                    BrandId = 1,
                    Brand = new ProductBrand { Id = 1, Name = "Brand A" },
                    CategoryId = 1,
                    Category = new ProductCategory { Id = 1, Name = "Category A" },
                    PictureUrl = "images/laptopA.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Laptop B",
                    Description = "Mid-range laptop",
                    Price = 1000m,
                    BrandId = 1,
                    Brand = new ProductBrand { Id = 1, Name = "Brand A" },
                    CategoryId = 1,
                    Category = new ProductCategory { Id = 1, Name = "Category A" },
                    PictureUrl = "images/laptopB.jpg"
                }
            };

            int totalCount = 2;

            _mockProductService.Setup(service => service.GetProductsAsync(specsParams))
                .ReturnsAsync(products);

            _mockProductService.Setup(service => service.GetCountAsync(specsParams))
                .ReturnsAsync(totalCount);

            // Act
            var result = await _controller.GetProducts(specsParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagination = Assert.IsType<Pagination<ProductToReturnDto>>(okResult.Value);

            // Verify pagination metadata
            Assert.Equal(specsParams.PageSize, pagination.PageSize);
            Assert.Equal(specsParams.PageIndex, pagination.PageIndex);
            Assert.Equal(totalCount, pagination.Count);

            // Verify data
            Assert.Equal(products.Count, pagination.Data.Count);
            Assert.Equal("Laptop A", pagination.Data[0].Name);
            Assert.Equal("Laptop B", pagination.Data[1].Name);
            Assert.Equal("Brand A", pagination.Data[0].Brand);
            Assert.Equal("Category A", pagination.Data[0].Category);
            Assert.Equal("https://localhost:7295/images/laptopA.jpg", pagination.Data[0].PictureUrl);
            Assert.Equal("https://localhost:7295/images/laptopB.jpg", pagination.Data[1].PictureUrl);
        }

        [Fact]
        public async Task GetProducts_WithNoMatchingProducts_ReturnsOkResult_WithEmptyData()
        {
            // Arrange
            var specsParams = new ProductSpecsParams
            {
                PageSize = 5,
                PageIndex = 1,
                BrandId = 999, // Assuming this brand ID does not exist
                CategoryId = 999,
                Search = "NonExistentProduct"
            };

            var products = new List<Product>(); // No matching products
            int totalCount = 0;

            _mockProductService.Setup(service => service.GetProductsAsync(specsParams))
                .ReturnsAsync(products);

            _mockProductService.Setup(service => service.GetCountAsync(specsParams))
                .ReturnsAsync(totalCount);

            // Act
            var result = await _controller.GetProducts(specsParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagination = Assert.IsType<Pagination<ProductToReturnDto>>(okResult.Value);

            // Verify pagination metadata
            Assert.Equal(specsParams.PageSize, pagination.PageSize);
            Assert.Equal(specsParams.PageIndex, pagination.PageIndex);
            Assert.Equal(totalCount, pagination.Count);

            // Verify data
            Assert.Empty(pagination.Data);
        }

        [Fact]
        public async Task GetProduct_ExistingId_ReturnsOkResult_WithProductDto()
        {
            // Arrange
            int productId = 1;
            var product = new Product
            {
                Id = productId,
                Name = "Laptop A",
                Description = "High-end laptop",
                Price = 1500m,
                BrandId = 1,
                Brand = new ProductBrand { Id = 1, Name = "Brand A" },
                CategoryId = 1,
                Category = new ProductCategory { Id = 1, Name = "Category A" },
                PictureUrl = "images/laptopA.jpg"
            };
            var productDto = _mapper.Map<ProductToReturnDto>(product);

            _mockProductService.Setup(service => service.GetProductAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductToReturnDto>(okResult.Value);
            returnedProduct.Should().BeEquivalentTo(productDto);
        }

        [Fact]
        public async Task GetProduct_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            int productId = 999; // Assuming this ID does not exist
            _mockProductService.Setup(service => service.GetProductAsync(productId))
                .ReturnsAsync((Product)null); // Simulate not found

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            apiResponse.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetBrands_ReturnsOkResult_WithListOfBrands()
        {
            // Arrange
            var brands = new List<ProductBrand>
            {
                new ProductBrand { Id = 1, Name = "Brand A" },
                new ProductBrand { Id = 2, Name = "Brand B" }
            };

            _mockProductService.Setup(service => service.GetBrandsAsync())
                .ReturnsAsync(brands);

            // Act
            var result = await _controller.GetBrands();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBrands = Assert.IsAssignableFrom<IReadOnlyList<ProductBrand>>(okResult.Value);
            returnedBrands.Should().HaveCount(brands.Count);
            returnedBrands.Should().BeEquivalentTo(brands);
        }

        [Fact]
        public async Task GetCategories_ReturnsOkResult_WithListOfCategories()
        {
            // Arrange
            var categories = new List<ProductCategory>
            {
                new ProductCategory { Id = 1, Name = "Category A" },
                new ProductCategory { Id = 2, Name = "Category B" }
            };

            _mockProductService.Setup(service => service.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsAssignableFrom<IReadOnlyList<ProductCategory>>(okResult.Value);
            returnedCategories.Should().HaveCount(categories.Count);
            returnedCategories.Should().BeEquivalentTo(categories);
        }
    }
}
