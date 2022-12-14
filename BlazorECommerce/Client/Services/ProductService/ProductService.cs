using BlazorECommerce.Shared;

namespace BlazorECommerce.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public event Action ProductsChanged;

    public List<Product> Products { get; set; } = new List<Product>();
    public string Messsage { get; set; } = "Loading Products...";
    public int CurrentPage { get; set; } = 1;
    public int PageCount { get; set; } = 0;
    public string LastSearchText { get; set; }

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task GetProducts(string? categoryUrl = null)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/{(categoryUrl == null ? "featured" : $"category/{categoryUrl}")}");
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

    public async Task SearchProducts(string searchText, int page = 1)
    {
        LastSearchText = searchText;
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"api/product/search/{searchText}/{page}");
        if (result != null && result.Data != null)
        {
            Products = result.Data.Products;
            CurrentPage = result.Data.CurretPage;
            PageCount = result.Data.Pages;
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
