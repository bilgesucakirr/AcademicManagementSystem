using Identity.Api.Data;
using Identity.Api.Dtos;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.Api.Services;

public interface IInvitationService
{
    Task<InvitationResponse> CreateInvitationAsync(InviteRequest request, string invitedByUserId);
    Task<bool> AcceptInvitationAsync(string token, string userId);
}

public class InvitationService : IInvitationService
{
    private readonly AppIdentityDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public InvitationService(AppIdentityDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<InvitationResponse> CreateInvitationAsync(InviteRequest request, string invitedByUserId)
    {
        // Rol kontrolü (Enum parse)
        if (!Enum.TryParse<TargetRole>(request.Role, true, out var roleEnum))
        {
            return new InvitationResponse { Success = false, Message = "Geçersiz rol." };
        }

        var token = Guid.NewGuid().ToString("N");
        var invitation = new Invitation
        {
            Email = request.Email,
            Role = roleEnum,
            TargetTrackId = request.TrackId,
            Token = token,
            InvitedByUserId = invitedByUserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Status = InvitationStatus.Pending
        };

        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync();

        // Demo linki (Gerçek hayatta e-posta ile gider)
        var link = $"https://localhost:7018/register?invitationToken={token}";

        Console.WriteLine($"[DAVETİYE OLUŞTURULDU] Kime: {request.Email}, Link: {link}");

        return new InvitationResponse { Success = true, InvitationLink = link, Message = "Davetiye oluşturuldu." };
    }

    public async Task<bool> AcceptInvitationAsync(string token, string userId)
    {
        var invite = await _context.Invitations
            .FirstOrDefaultAsync(i => i.Token == token && i.Status == InvitationStatus.Pending);

        if (invite == null || invite.ExpiresAt < DateTime.UtcNow) return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        // 1. Rolü Ata
        var roleName = invite.Role.ToString();
        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        // 2. Eğer Track Chair ise, yetki alanını (Claim) ekle
        if (invite.Role == TargetRole.TrackChair && invite.TargetTrackId.HasValue)
        {
            await _userManager.AddClaimAsync(user, new Claim("AssignedTrackId", invite.TargetTrackId.Value.ToString()));
        }

        // 3. Davetiyeyi kullanıldı olarak işaretle
        invite.Status = InvitationStatus.Accepted;
        await _context.SaveChangesAsync();

        return true;
    }
}