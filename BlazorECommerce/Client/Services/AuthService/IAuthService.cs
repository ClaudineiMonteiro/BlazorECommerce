namespace BlazorECommerce.Client.Services.AuthService;

public interface IAuthService
{
    Task<ServiceResponse<int>> Register(UserRegister userRegister);
    Task<ServiceResponse<string>> Login(UserLogin userLogin);
    Task<ServiceResponse<bool>> Logout();
    Task<ServiceResponse<bool>> ChangePassword(UserChangePassword userChangePassword);
}
