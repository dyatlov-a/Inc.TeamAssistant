using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;

public sealed record ShowHelpQuery(LanguageId LanguageId) : IRequest<ShowHelpResult>, IWithLanguage;