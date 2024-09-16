using System.Text.Json;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedData(StoreContext context)
        {
            if (context.Brands.Count() == 0)
            {
                var brandData = File.ReadAllText("../Talabat.Repository/SeedData/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
                if (brands?.Count() > 0)
                {
                    foreach (var brand in brands)
                    {
                        context.Set<ProductBrand>().Add(brand);
                    }
                    await context.SaveChangesAsync();
                } 
            }

            if (context.Categories.Count() == 0)
            {
                var CategoriesData = File.ReadAllText("../Talabat.Repository/SeedData/categories.json");
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(CategoriesData);
                if (categories?.Count() > 0)
                {
                    foreach (var category in categories)
                    {
                        context.Set<ProductCategory>().Add(category);
                    }
                    await context.SaveChangesAsync();
                }
            }

            if (context.Products.Count() == 0)
            {
                var productsData = File.ReadAllText("../Talabat.Repository/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        context.Set<Product>().Add(product);
                    }
                    await context.SaveChangesAsync();
                }
            }
            
            if (context.DeliveryMethods.Count() == 0)
            {
                var deliveryMethodsData = File.ReadAllText("../Talabat.Repository/SeedData/delivery.json");
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
                if (deliveryMethods?.Count() > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        context.Set<DeliveryMethod>().Add(deliveryMethod);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
