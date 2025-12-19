using MediatR;

namespace Review.Application.Features.Assignments.Commands.DeclineInvitation;

public record DeclineInvitationCommand(Guid AssignmentId, string Reason) : IRequest<Unit>;