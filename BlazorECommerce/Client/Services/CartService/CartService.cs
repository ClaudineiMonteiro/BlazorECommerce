using BlazorECommerce.Client.Pages;
using BlazorECommerce.Shared;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace BlazorECommerce.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public event Action OnChange;
    public CartService(ILocalStorageService localStorage,
        HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }
    public async Task AddToCart(CartItem cartItem)
    {
        if (await IsUserAuthenticated())
        {
            Console.WriteLine("User is Authenticated");
        }
        else
        {
            Console.WriteLine("User is Not Authenticated");
        }

        List<CartItem>? cart = await GetFromLocalStorage();

        var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);
        if (sameItem == null)
        {
            cart.Add(cartItem);
        }
        else
        {
            sameItem.Quantity += cartItem.Quantity;
        }

        await _localStorage.SetItemAsync("cart", cart);
        await GetCartItemsCount();
    }

    private async Task<bool> IsUserAuthenticated()
    {
        return (await _authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
    }

    private async Task<List<CartItem>> GetFromLocalStorage()
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null)
        {
            cart = new List<CartItem>();
        }

        return cart;
    }

    public async Task<List<CartItem>> GetCartItems()
    {
        await GetCartItemsCount();
        List<CartItem>? cart = await GetFromLocalStorage();
        if (cart == null)
        {
            cart= new List<CartItem>();
        }
        return cart;
    }

    public async Task<List<CartProductResponse>> GetCartProducts()
    {
        var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
        var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
        return cartProducts.Data;


    }

    public async Task RemoveProductFromCart(int productId, int productTypeId)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null) return;
        var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);
        if (cartItem != null)
        {
            cart.Remove(cartItem);
            await _localStorage.SetItemAsync("cart", cart);
            await GetCartItemsCount();
        }
    }

    public async Task UpdateQuantity(CartProductResponse product)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null) return;
        var cartItem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);
        if (cartItem != null)
        {
            cartItem.Quantity = product.Quantity;
            await _localStorage.SetItemAsync("cart", cart);
        }
    }

    public async Task StorageCartItems(bool emptyLocalCart = false)
    {
        var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (localCart == null) return;

        await _httpClient.PostAsJsonAsync("api/cart", localCart);

        if (emptyLocalCart)
        {
            await _localStorage.RemoveItemAsync("cart");
        }
    }

    public async Task GetCartItemsCount()
    {
        int count = 0;
        if (await IsUserAuthenticated())
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
            count = result.Data;
        }
        else
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart != null)
            {
                count = cart.Count;
            }
        }
        await _localStorage.SetItemAsync<int>("cartItemsCount", count);
        OnChange.Invoke();
    }
}
