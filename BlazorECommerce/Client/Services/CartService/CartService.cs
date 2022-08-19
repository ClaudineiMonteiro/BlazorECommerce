using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace BlazorECommerce.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public event Action OnChange;
    public CartService(ILocalStorageService localStorage,
        HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }
    public async Task AddToCart(CartItem cartItem)
    {
        List<CartItem>? cart = await GetFromLocalStorage();
        cart.Add(cartItem);
        await _localStorage.SetItemAsync("cart", cart);
        OnChange?.Invoke();
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
        List<CartItem>? cart = await GetFromLocalStorage();
        if (cart == null)
        {
            cart= new List<CartItem>();
        }
        return cart;
    }

    public async Task<List<CartProductResponse>> GetAllCartProducts()
    {
        var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
        var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
        return cartProducts.Data;


    }
}
