using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;

public sealed record ChangeLanguageCommand(
        long TargetChatId,
        ParticipantId ModeratorId,
        string ModeratorName,
        LanguageId LanguageId)
    : IRequest<CommandResult>, IWithModerator, IWithLanguage;