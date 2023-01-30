using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;

public sealed record AddStoryToAssessmentSessionCommand(
    LanguageId LanguageId,
    ParticipantId ModeratorId,
    string ModeratorName,
    string Title,
    IReadOnlyCollection<string> Links) : IRequest<AddStoryToAssessmentSessionResult>, IWithModerator, IWithLanguage;