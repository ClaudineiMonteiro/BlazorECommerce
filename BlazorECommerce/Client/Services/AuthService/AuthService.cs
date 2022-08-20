namespace BlazorECommerce.Client.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<int>> Register(UserRegister userRegister)
    {
        var result = await _http.PostAsJsonAsync("api/auth/register", userRegister);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>() ?? new ServiceResponse<int> { Success = false, Message = "Some problem some problem occuored!!!" };
    }
}
