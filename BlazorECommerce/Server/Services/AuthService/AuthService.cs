
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BlazorECommerce.Server.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(DataContext context, 
                       IConfiguration configuration,
                       IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    public async Task<ServiceResponse<string>> Login(string email, string password)
    {
        var response = new ServiceResponse<string>();
        var user = await _context.Users.FirstOrDefaultAsync(c => c.Email.ToLower().Equals(email.ToLower()));
        if (user == null)
        {
            response.Success = false;
            response.Message = "User not found.";
        }
        else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Wrong password.";
        }
        else
        {
            response.Data = CreateToken(user);
        }

        return response;
    }

    private string CreateToken(User user)
    {
        List<Claim> claimsList = new List<Claim> 
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email)
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken
            (
                claims: claimsList,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return hash.SequenceEqual(passwordHash);
        }
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        if (await UserExists(user.Email))
        {
            return new ServiceResponse<int> 
            { 
                Success = false,
                Message = "User already exists."
            };
        }

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _context.Users.Add(user);
        _context.SaveChanges();

        return new ServiceResponse<int> { Data = user.Id, Message = "Registration successful!!!" };
    }

    public async Task<bool> UserExists(string email)
    {
        return await _context.Users.AnyAsync(c => c.Email.ToLower().Equals(email.ToLower()));
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "User Not found."
            };
        }

        CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        
        user.PasswordSalt = passwordSalt;
        user.PasswordHash = passwordHash;
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "Password has been changed"
        };
    }
}
