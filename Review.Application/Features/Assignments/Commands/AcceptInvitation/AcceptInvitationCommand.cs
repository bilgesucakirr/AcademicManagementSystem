using MediatR;

namespace Review.Application.Features.Assignments.Commands.AcceptInvitation;

public record AcceptInvitationCommand(Guid AssignmentId) : IRequest<Unit>;