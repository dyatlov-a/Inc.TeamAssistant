using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

internal sealed class LinkBuilder : ILinkBuilder
{
    private readonly string _botLink;
    private readonly string _linkForConnectTemplate;
    private readonly string _linkForDashboardTemplate;

    public LinkBuilder(string botLink, string linkForConnectTemplate, string linkForDashboardTemplate)
    {
        if (string.IsNullOrWhiteSpace(botLink))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botLink));
        if (string.IsNullOrWhiteSpace(linkForConnectTemplate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkForConnectTemplate));
        if (string.IsNullOrWhiteSpace(linkForDashboardTemplate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkForDashboardTemplate));

        _botLink = botLink;
        _linkForConnectTemplate = linkForConnectTemplate;
        _linkForDashboardTemplate = linkForDashboardTemplate;
    }

    public string BuildLinkMoveToBot() => _botLink;

    public string BuildLinkForConnect(AssessmentSessionId assessmentSessionId)
    {
        if (assessmentSessionId is null)
            throw new ArgumentNullException(nameof(assessmentSessionId));

        return string.Format(_linkForConnectTemplate, _botLink, assessmentSessionId.Value.ToString("N"));
    }

    public string BuildLinkForDashboard(AssessmentSessionId assessmentSessionId, LanguageId assessmentSessionLanguage)
    {
        if (assessmentSessionId is null)
            throw new ArgumentNullException(nameof(assessmentSessionId));

        return string.Format(
            _linkForDashboardTemplate,
            assessmentSessionLanguage.Value,
            assessmentSessionId.Value.ToString("N"));
    }
}