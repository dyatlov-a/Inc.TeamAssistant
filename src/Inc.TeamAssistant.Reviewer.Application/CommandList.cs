namespace Inc.TeamAssistant.Reviewer.Application;

internal static class CommandList
{
    public const string MoveToReview = "/move_to_review?draftId=";
    public const string NeedReview = "/need_review";
    public const string MoveToInProgress = "/in_progress?storyId=";
    public const string Accept = "/approve?storyId=";
    public const string Decline = "/decline?storyId=";
    public const string MoveToNextRound = "/next_round?storyId=";
    public const string ReassignReview = "/reassign?storyId=";
    public const string RemoveDraft = "/remove_draft?draftId=";
    public const string EditDraft = "/edit_draft?description=";
    public const string ChangeToRoundRobin = "/change_to_round_robin";
    public const string ChangeToRandom = "/change_to_random";
}