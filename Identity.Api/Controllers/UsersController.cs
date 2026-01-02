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

        var roleExists = await _userManager.IsInRoleAsync(user, request.Role);
        if (roleExists) return BadRequest("User already has this role.");

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new { Message = $"Role {request.Role} assigned to {user.FullName}" });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new
            {
                user.Id,
                user.FullName,
                user.Email,
                Roles = roles
            });
        }
        return Ok(userList);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        var profile = new UserProfileDto
        {
            Email = user.Email!,
            FullName = user.FullName,
            Interests = user.Interests,
            Roles = roles
        };

        return Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found.");

        user.FullName = request.FullName;
        user.Interests = request.Interests;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "Profile updated successfully." });
    }
}