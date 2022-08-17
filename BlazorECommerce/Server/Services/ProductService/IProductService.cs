﻿namespace BlazorECommerce.Server.Services.ProductService;

public interface IProductService
{
    Task<ServiceResponse<List<Product>>> GetAllProductsAsync();
    Task<ServiceResponse<Product>> GetProductAsync(int productId);
    Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl);
    Task<ServiceResponse<List<Product>>> SearchProducts(string searchText);
    Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText);
    Task<ServiceResponse<List<Product>>> GetFeaturedProducts();
}
