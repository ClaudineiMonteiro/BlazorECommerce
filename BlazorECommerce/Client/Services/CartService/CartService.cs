﻿using Blazored.LocalStorage;

namespace BlazorECommerce.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;

    public event Action OnChange;
    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
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
}
