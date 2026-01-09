using Identity.Api.Dtos;
using Identity.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("reviewers")]
    [Authorize(Roles = "Admin,EditorInChief,TrackChair")]
    public async Task<IActionResult> GetReviewers([FromQuery] string? keyword)
    {
        var allReviewers = await _userManager.GetUsersInRoleAsync("Reviewer");
        var query = allReviewers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(u => u.Interests.ToLower().Contains(lowerKeyword) ||
                                     u.FullName.ToLower().Contains(lowerKeyword));
        }

        var result = query.Select(u => new
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Interests = u.Interests
        });

        return Ok(result);
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return NotFound("User not found.");

        if (!await _userManager.IsInRoleAsync(user, request.Role))
        {
            await _userManager.AddToRoleAsync(user, request.Role);
        }

        return Ok(new { Message = $"Role {request.Role} assigned." });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var list = new List<object>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            list.Add(new { u.Id, u.FullName, u.Email, Roles = roles, u.Interests });
        }
        return Ok(list);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserProfileDto
        {
            Email = user.Email!,
            FullName = user.FullName,
            Interests = user.Interests,
            Roles = roles
        });
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return NotFound();

        user.FullName = request.FullName;
        user.Interests = request.Interests;

        await _userManager.UpdateAsync(user);
        return Ok();
    }

    [HttpPost("set-author-role/{userId}")]
    [Authorize]
    public async Task<IActionResult> SetAuthorRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        if (!await _userManager.IsInRoleAsync(user, "Author"))
        {
            await _userManager.AddToRoleAsync(user, "Author");
        }
        return Ok();
    }
}