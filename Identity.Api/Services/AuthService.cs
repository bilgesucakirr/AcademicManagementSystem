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
    private readonly IInvitationService _invitationService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IInvitationService invitationService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _invitationService = invitationService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
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
            return new AuthResponse { Success = false };

        if (request is RegisterWithInviteRequest inviteRequest &&
            !string.IsNullOrEmpty(inviteRequest.InvitationToken))
        {
            await _invitationService.AcceptInvitationAsync(inviteRequest.InvitationToken, user.Id);
        }

        var token = await GenerateJwtToken(user);

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
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return new AuthResponse { Success = false };

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return new AuthResponse { Success = false };

        var token = await GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Token = token,
            UserId = user.Id,
            FullName = user.FullName
        };
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                double.Parse(jwtSettings["DurationInMinutes"]!)),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
