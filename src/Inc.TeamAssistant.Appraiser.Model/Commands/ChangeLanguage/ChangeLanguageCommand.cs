using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;

public sealed record ChangeLanguageCommand(ParticipantId ModeratorId, string ModeratorName, LanguageId LanguageId)
    : IRequest<ChangeLanguageResult>, IWithModerator, IWithLanguage;