namespace Inc.TeamAssistant.Connector.Application;

internal static class CommandList
{
    public const string Start = "/start";
    public const string Cancel = "/cancel";
    public const string NewTeam = "/new_team";
    public const string LeaveTeam = "/leave_team";
    public const string RemoveTeam = "/remove_team";
    public const string Help = "/help";
    
    public const string MoveToFibonacci = "/move_to_fibonacci";
    public const string MoveToTShirts = "/move_to_tshirts";
    public const string MoveToPowerOfTwo = "/move_to_power_of_two";
    public const string AddStory = "/add";

    public const string ChangeToRoundRobin = "/change_to_round_robin";
    public const string ChangeToRandom = "/change_to_random";

    private const string NeedReviewAlias = "/nr";
    public const string NeedReview = "/need_review";

    private const string AddLocationAlias = "/l";
    public const string AddLocation = "/location";

    public const string AddPollAnswer = "/poll_answer?pollId={0}&option={1}";
    
    public const string EditDraft = "/edit_draft?description={0}";

    public static IEnumerable<(string Aleas, string Command)> BuildAliasMap()
    {
        yield return (NeedReviewAlias, NeedReview);
        yield return (AddLocationAlias, AddLocation);
    }
}