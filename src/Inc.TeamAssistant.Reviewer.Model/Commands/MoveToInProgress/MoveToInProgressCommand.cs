using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;

public sealed record MoveToInProgressCommand(Guid TaskId, LanguageId PersonLanguageId) : IRequest;