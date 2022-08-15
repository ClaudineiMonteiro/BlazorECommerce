using BlazorECommerce.Shared;

namespace BlazorECommerce.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public List<Product> Products { get; set; } = new List<Product>();

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task GetProducts()
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");
        if (result != null && result.Data != null)
        {
            Products = result.Data;
        }
    }
}
