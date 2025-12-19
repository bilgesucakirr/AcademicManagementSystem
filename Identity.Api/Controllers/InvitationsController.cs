using Identity.Api.Dtos;
using Identity.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvitationsController : ControllerBase
{
    private readonly IInvitationService _invitationService;

    public InvitationsController(IInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    [HttpPost("invite")]
    [Authorize(Roles = "Admin, EditorInChief")] // Only Admins/EIC can invite
    public async Task<IActionResult> Invite([FromBody] InviteRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _invitationService.CreateInvitationAsync(request, userId!);

        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("accept")]
    [Authorize] // Must be logged in to accept (even if just registered)
    public async Task<IActionResult> Accept([FromQuery] string token)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var success = await _invitationService.AcceptInvitationAsync(token, userId!);

        if (!success) return BadRequest("Invalid or expired invitation.");
        return Ok("Invitation accepted. Role assigned.");
    }
}