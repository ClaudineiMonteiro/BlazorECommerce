using Microsoft.AspNetCore.Mvc;

namespace BlazorECommerce.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("register")]
	public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegister userRegister)
	{
		var response = await _authService.Register(new User { Email = userRegister.Email }, userRegister.Password);
		if (!response.Success)
		{
			return BadRequest(response);
		}

		return Ok(response);
	}

	[HttpPost("login")]
	public async Task<ActionResult<ServiceResponse<string>>> Login(UserLogin userLogin)
	{
		var response = await _authService.Login(userLogin.Email, userLogin.Password);
		if (!response.Success)
		{
			return BadRequest(response);
		}

		return Ok(response);
	}
}
