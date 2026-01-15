namespace Identity.Api.Dtos;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class AssignRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;

    // YENİ
    public string? Affiliation { get; set; }
    public string? Country { get; set; }
    public string? Biography { get; set; }
    public string? Title { get; set; }
}


public class UserProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }

    // YENİ
    public string? Affiliation { get; set; }
    public string? Country { get; set; }
    public string? Biography { get; set; }
    public string? Title { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}