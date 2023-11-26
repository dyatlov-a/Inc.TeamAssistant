using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;

public sealed record AddStoryToAssessmentSessionCommand(
    LanguageId LanguageId,
    long ModeratorId,
    string ModeratorName,
    string Title,
    IReadOnlyCollection<string> Links) : IRequest<CommandResult>, IWithModerator, IWithLanguage;