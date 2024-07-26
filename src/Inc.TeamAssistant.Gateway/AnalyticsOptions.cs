namespace Inc.TeamAssistant.Gateway;

public sealed class AnalyticsOptions
{
    public string SentryDsn { get; set; } = default!;
    public string? GoogleVerificationValue { get; set; }
    public string? YandexVerificationValue { get; set; }
}