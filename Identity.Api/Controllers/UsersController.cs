using Identity.Api.Dtos;
using Identity.Api.Models;
using Identity.Api.Services; // FileService için namespace
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
    private readonly IFileService _fileService; // Eklendi

    public UsersController(UserManager<ApplicationUser> userManager, IFileService fileService)
    {
        _userManager = userManager;
        _fileService = fileService;
    }

    // --- Profil Fotoğrafı Yükleme ---
    [HttpPost("profile-picture")]
    [Authorize]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return NotFound();

        try
        {
            var url = await _fileService.SaveProfilePictureAsync(file);
            user.ProfilePictureUrl = url;
            await _userManager.UpdateAsync(user);
            return Ok(new { Url = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
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
            ProfilePictureUrl = user.ProfilePictureUrl,
            Affiliation = user.Affiliation,
            Country = user.Country,
            Biography = user.Biography,
            Title = user.Title,
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
        user.Affiliation = request.Affiliation;
        user.Country = request.Country;
        user.Biography = request.Biography;
        user.Title = request.Title;

        await _userManager.UpdateAsync(user);
        return Ok();
    }


    [HttpGet("reviewers")]
    [Authorize(Roles = "Admin,EditorInChief,TrackChair")]
    public async Task<IActionResult> GetReviewers([FromQuery] string? keyword, [FromQuery] string? interest)
    {
        var allReviewers = await _userManager.GetUsersInRoleAsync("Reviewer");
        var query = allReviewers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(u => u.Interests.ToLower().Contains(lowerKeyword) ||
                                     u.FullName.ToLower().Contains(lowerKeyword));
        }

        if (!string.IsNullOrWhiteSpace(interest))
        {
            query = query.Where(u => u.Interests.Contains(interest, StringComparison.OrdinalIgnoreCase));
        }

        var result = query.Select(u => new
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Interests = u.Interests,
            ProfilePictureUrl = u.ProfilePictureUrl // Admin/Editör görebilsin
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
            list.Add(new { u.Id, u.FullName, u.Email, Roles = roles, u.Interests, u.ProfilePictureUrl });
        }
        return Ok(list);
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

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });

        return Ok(new { Message = "Password changed successfully." });
    }

}