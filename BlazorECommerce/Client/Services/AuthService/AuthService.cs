using BlazorECommerce.Shared;

namespace BlazorECommerce.Client.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword userChangePassword)
    {
        var result = await _http.PostAsJsonAsync("api/auth/change-password", userChangePassword.Password);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<string>> Login(UserLogin userLogin)
    {
        var result = await _http.PostAsJsonAsync("api/auth/login", userLogin);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>() ?? new ServiceResponse<string> { Success = false, Message = "Some problem some problem occuored!!!" };
    }

    public Task<ServiceResponse<bool>> Logout()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<int>> Register(UserRegister userRegister)
    {
        var result = await _http.PostAsJsonAsync("api/auth/register", userRegister);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>() ?? new ServiceResponse<int> { Success = false, Message = "Some problem some problem occuored!!!" };
    }
}
