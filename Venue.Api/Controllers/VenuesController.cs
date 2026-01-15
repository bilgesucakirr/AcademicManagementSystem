using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Venue.Api.Services; // FileService namespace
using Venue.Application.Features.Venues.Commands.CreateVenue;
using Venue.Application.Features.Venues.Queries.GetAllVenues;
using Venue.Application.Features.Venues.Queries.GetVenueById;

namespace Venue.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VenuesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IFileService _fileService; // Servis Inject Edildi

    public VenuesController(ISender mediator, IFileService fileService)
    {
        _mediator = mediator;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllVenuesQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetVenueByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,EditorInChief")]
    public async Task<IActionResult> Create([FromBody] CreateVenueCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPost("upload-form")]
    [Authorize(Roles = "Admin,EditorInChief")]
    public async Task<IActionResult> UploadForm(IFormFile file)
    {
        try
        {
            var url = await _fileService.SaveFileAsync(file);
            return Ok(new { Url = url });
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }
}