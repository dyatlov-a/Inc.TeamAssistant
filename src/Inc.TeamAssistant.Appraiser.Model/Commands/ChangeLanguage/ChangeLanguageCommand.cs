using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;

public sealed record ChangeLanguageCommand(
        long TargetChatId,
        long ModeratorId,
        string ModeratorName,
        LanguageId LanguageId)
    : IRequest<CommandResult>, IWithModerator, IWithLanguage;