using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface ILinkBuilder
{
    string BuildLinkMoveToBot();

    string BuildLinkForConnect(Guid assessmentSessionId);

    string BuildLinkForDashboard(Guid assessmentSessionId, LanguageId assessmentSessionLanguageId);
}