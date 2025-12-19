namespace Identity.Api.Dtos;

public class InviteRequest
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; 
    public int? TrackId { get; set; }
}

public class InvitationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string InvitationLink { get; set; } = string.Empty; 
}

public class RegisterWithInviteRequest : RegisterRequest
{
    public string? InvitationToken { get; set; }
}