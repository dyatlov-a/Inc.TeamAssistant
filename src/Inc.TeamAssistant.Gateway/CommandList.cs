namespace Inc.TeamAssistant.Gateway;

internal static class CommandList
{
    public const string Start = "/start";
    public const string JoinToSession = "/join";
    public const string AllowUseName = "/allow_use_name";
    public const string CreateAssessmentSession = "/new";
    public const string ChangeLanguageForAssessmentSession = "/{0}";
    public const string ShowParticipants = "/users";
    public const string AddStoryToAssessmentSession = "/add";
    public const string AcceptEstimate = "/accept";
    public const string ReVoteEstimate = "/revote";
    public const string FinishAssessmentSession = "/finish";

    public const string Help = "/help";
    public const string ExitFromAssessmentSession = "/exit";
}