using Microsoft.AspNetCore.Components;

namespace BlazorECommerce.Client.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager _navigationManager;

    public OrderService(HttpClient httpClient,
                        AuthenticationStateProvider authenticationStateProvider,
                        NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task PlaceOrder()
    {
        if (await IsUserAuthenticated())
        {
            await _httpClient.PostAsync("api/order", null);
        }
        else
        {
            _navigationManager.NavigateTo("loing");
        }

    }

    private async Task<bool> IsUserAuthenticated()
    {
        return (await _authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
    }
}
