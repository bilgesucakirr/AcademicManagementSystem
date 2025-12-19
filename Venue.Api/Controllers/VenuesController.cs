using MediatR;
using Microsoft.AspNetCore.Mvc;
using Venue.Application.Features.Venues.Queries.GetAllVenues;

namespace Venue.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VenuesController : ControllerBase
{
    private readonly ISender _mediator;

    public VenuesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllVenuesQuery());
        return Ok(result);
    }
}