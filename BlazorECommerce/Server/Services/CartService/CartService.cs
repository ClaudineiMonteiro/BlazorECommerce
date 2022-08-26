using System.Security.Claims;

namespace BlazorECommerce.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(DataContext context,
                       IAuthService authService)
    {
        _context = context;
        _authService = authService;
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
                ProductTypeId = productVariant.ProductTypeId,
                Quantity = item.Quantity

            };

            result.Data.Add(cartProduct);
        }
        return result;
    }

    public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
    {
        int userId = _authService.GetUserId();
        cartItems.ForEach(cartItem => cartItem.UserId = userId);
        _context.CarItems.AddRange(cartItems);
        await _context.SaveChangesAsync();

        return await GetDbCartProducts();
    }

    public async Task<ServiceResponse<int>> GetCartItemsCount()
    {
        try
        {
            var userId = _authService.GetUserId();
            var cartItemsCount = await _context.CarItems.Where(ci => ci.UserId == userId).ToListAsync();
            var count = cartItemsCount.Count();
            return new ServiceResponse<int> { Data = count };

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts()
    {
        return await GetCartProducts(await _context.CarItems.Where(ci => ci.UserId == _authService.GetUserId()).ToListAsync());
    }

    public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
    {
        cartItem.UserId = _authService.GetUserId();

        var sameItem = await _context.CarItems
            .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
                ci.ProductTypeId == cartItem.ProductTypeId &&
                ci.UserId == cartItem.UserId);

        if (sameItem == null)
        {
            _context.CarItems.Add(cartItem);
        }
        else
        {
            sameItem.Quantity += cartItem.Quantity;
        }
        await _context.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem)
    {
        var dbCartItem = await _context.CarItems
            .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
                ci.ProductTypeId == cartItem.ProductTypeId &&
                ci.UserId == _authService.GetUserId());
        if (dbCartItem == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exist."
            };
        }
        
        dbCartItem.Quantity = cartItem.Quantity;
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
    {
        var itemRemoved = _context.CarItems
                .FirstOrDefault(ci => ci.ProductId == productId && ci.ProductTypeId == productTypeId && ci.UserId == _authService.GetUserId());
        if (itemRemoved == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exists."
            };
        }
        _context.CarItems.Remove(itemRemoved);
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }
}
