using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ShowHelp;

public sealed record ShowHelpCommand(long TargetChatId, LanguageId LanguageId)
    : IRequest<CommandResult>, IWithLanguage;