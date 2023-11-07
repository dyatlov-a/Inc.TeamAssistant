using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;

public sealed record ShowHelpQuery(long TargetChatId, LanguageId LanguageId)
    : IRequest<CommandResult>, IWithLanguage;