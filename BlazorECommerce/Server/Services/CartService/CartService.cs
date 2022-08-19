namespace BlazorECommerce.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;

    public CartService(DataContext context)
    {
        _context = context;
    }
    public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = new ServiceResponse<List<CartProductResponse>>
        {
            Data = new List<CartProductResponse>()
        };

        foreach (var item in cartItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(c => c.Id == item.ProductId);
            if (product == null)
            {
                continue;
            }

            var productVariant = await _context.ProductVariants.FirstOrDefaultAsync(c => c.ProductId == item.ProductId && c.ProductTypeId == item.ProductTypeId);
            if (productVariant == null)
            {
                continue;
            }

            var cartProduct = new CartProductResponse
            {
                ProductId = product.Id,
                Title = product.Title,
                ImageUrl = product.ImageUrl,
                Price = productVariant.Price,
                ProductType = productVariant?.ProductType?.Name,
                ProductTypeId = productVariant.ProductTypeId
            };

            result.Data.Add(cartProduct);
        }
        return result;
    }
}
