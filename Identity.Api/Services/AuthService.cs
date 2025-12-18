using Identity.Api.Dtos;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {

        Console.WriteLine($"[KAYIT İSTEĞİ] Ad: '{request.FullName}' | Email: '{request.Email}' | Şifre: '{request.Password}'");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return new AuthResponse { Success = false, Message = "Bu e-posta adresi zaten kullanımda." };

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse { Success = false, Message = "Kayıt başarısız: " + errors };
        }

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Success = true,
            Token = token,
            UserId = user.Id,
            FullName = user.FullName
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // --- DEBUG LOGLARI BAŞLANGIÇ ---
        // Bu loglar Identity.Api Output penceresinde görünür.
        Console.WriteLine($"[LOGIN İSTEĞİ] Email: '{request.Email}' | Şifre: '{request.Password}'");

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            Console.WriteLine($"[LOGIN BAŞARISIZ] Kullanıcı veritabanında bulunamadı: '{request.Email}'");
            return new AuthResponse { Success = false, Message = "Kullanıcı bulunamadı veya şifre hatalı." };
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            Console.WriteLine($"[LOGIN BAŞARISIZ] Şifre hatalı: '{request.Email}'");
            return new AuthResponse { Success = false, Message = "Kullanıcı bulunamadı veya şifre hatalı." };
        }

        Console.WriteLine($"[LOGIN BAŞARILI] Giriş onaylandı: '{request.Email}'");
        // --- DEBUG LOGLARI BİTİŞ ---

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Token = token,
            UserId = user.Id,
            FullName = user.FullName
        };
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}