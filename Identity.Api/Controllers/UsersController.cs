using Identity.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> GetReviewers()
    {
        var reviewers = await _userManager.GetUsersInRoleAsync("Reviewer");

        var result = reviewers.Select(u => new
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email
        });

        return Ok(result);
    }
}