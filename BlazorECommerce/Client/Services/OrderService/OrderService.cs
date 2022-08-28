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

    public async Task<OrderDetailsResponse> GetOrderDetails(int orderId)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<OrderDetailsResponse>>($"api/Order/{orderId}");
        return result.Data;
    }

    public async Task<List<OrderOverviewResponse>> GetOrders()
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponse>>>("api/order");
        return result.Data;
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
