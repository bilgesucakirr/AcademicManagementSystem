using MediatR;
using Microsoft.AspNetCore.Mvc;
using Submission.Application.Features.Integration;

namespace Submission.Api.Controllers;

[Route("api/integration")]
[ApiController]
public class IntegrationController : ControllerBase
{
    private readonly ISender _mediator;
    private const string ApiKey = "INTERNAL_SECRET_KEY_123";

    public IntegrationController(ISender mediator) => _mediator = mediator;

    [HttpPost("submissions/{id}/reviewer-decision")]
    public async Task<IActionResult> ApplyReviewerDecision(Guid id, [FromBody] ReviewerDecisionPayload payload)
    {
        if (!Request.Headers.TryGetValue("X-Internal-Key", out var key) || key != ApiKey)
            return Unauthorized();

        await _mediator.Send(new ProcessReviewerDecisionCommand(id, payload.Recommendation));
        return Ok();
    }
}
public record ReviewerDecisionPayload(string Recommendation);