using BlazorECommerce.Shared;

namespace BlazorECommerce.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public event Action ProductsChanged;

    public List<Product> Products { get; set; } = new List<Product>();
    public string Messsage { get; set; } = "Loading Products...";

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task GetProducts(string? categoryUrl = null)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product{(categoryUrl == null ? string.Empty : $"/category/{categoryUrl}")}");
        if (result != null && result.Data != null)
        {
            Products = result.Data;
        }

        ProductsChanged.Invoke();
    }

    public async Task<ServiceResponse<Product>> GetProduct(int productId)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"api/Product/{productId}");
        return result;
    }

    public async Task SearchProducts(string searchText)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/search/{searchText}");
        if (result != null && result.Data != null)
        {
            Products = result.Data;
        }
        if (Products.Count == 0) Messsage = "No products found.";
        ProductsChanged.Invoke();
    }

    public async Task<List<string>> GetAllProductSearchSuggestions(string searchText)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");
        return result.Data;


    }
}
