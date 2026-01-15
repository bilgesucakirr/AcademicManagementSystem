using Identity.Api.Data;
using Identity.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InterestsController : ControllerBase
{
    private readonly AppIdentityDbContext _context;

    public InterestsController(AppIdentityDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.AreasOfInterest.OrderBy(a => a.Name).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> Add([FromBody] string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return BadRequest("Name is required");

        if (await _context.AreasOfInterest.AnyAsync(a => a.Name == name))
            return BadRequest("This interest already exists.");

        _context.AreasOfInterest.Add(new AreaOfInterest { Name = name });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.AreasOfInterest.FindAsync(id);
        if (item == null) return NotFound();

        _context.AreasOfInterest.Remove(item);
        await _context.SaveChangesAsync();
        return Ok();
    }
}