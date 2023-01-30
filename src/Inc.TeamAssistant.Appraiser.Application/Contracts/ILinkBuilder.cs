using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface ILinkBuilder
{
    string BuildLinkMoveToBot();

    string BuildLinkForConnect(AssessmentSessionId assessmentSessionId);

    string BuildLinkForDashboard(AssessmentSessionId assessmentSessionId, LanguageId assessmentSessionLanguageId);
}