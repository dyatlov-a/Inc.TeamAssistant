using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;

public sealed record MoveToDeclineCommand(Guid TaskId, LanguageId PersonLanguageId) : IRequest;