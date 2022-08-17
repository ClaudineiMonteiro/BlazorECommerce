using Microsoft.AspNetCore.Mvc;

namespace BlazorECommerce.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{productId:int}")]
    public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int productId)
    {
        var products = await _productService.GetProductAsync(productId);
        return Ok(products);
    }

    [HttpGet("category/{categoryUrl}")]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string categoryUrl)
    {
        var result = await _productService.GetProductsByCategory(categoryUrl);
        return Ok(result);
    }

    [HttpGet("search/{searchText}")]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> SearchProducts(string searchText)
    {
        var result = await _productService.SearchProducts(searchText);
        return Ok(result);
    }

    [HttpGet("searchsuggestions/{searchText}")]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductSearchSuggestions(string searchText)
    {
        var result = await _productService.GetProductSearchSuggestions(searchText);
        return Ok(result);
    }
}
