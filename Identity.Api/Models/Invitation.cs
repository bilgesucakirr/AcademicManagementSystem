using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Models;

public enum InvitationStatus { Pending, Accepted, Declined, Expired }
public enum TargetRole { Reviewer, TrackChair }

public class Invitation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public TargetRole Role { get; set; }
    public int? TargetTrackId { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string InvitedByUserId { get; set; } = string.Empty;
}