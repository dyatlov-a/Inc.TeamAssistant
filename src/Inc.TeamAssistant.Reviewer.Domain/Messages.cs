using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Reviewer.Domain;

internal static class Messages
{
    public static readonly MessageId Connector_HasNoRights = new(nameof(Connector_HasNoRights));
    public static readonly MessageId Reviewer_TargetReassigned = new(nameof(Reviewer_TargetReassigned));
    public static readonly MessageId Reviewer_TargetManually = new(nameof(Reviewer_TargetManually));
    public static readonly MessageId Reviewer_TargetAutomatically = new(nameof(Reviewer_TargetAutomatically));
}