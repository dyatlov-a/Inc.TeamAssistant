using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface ILinkBuilder
{
    string BuildLinkMoveToBot();

    string BuildLinkForConnect(Guid teamId);

    string BuildLinkForDashboard(Guid teamId, LanguageId languageId);
}