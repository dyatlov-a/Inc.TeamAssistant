namespace Inc.TeamAssistant.Connector.Application;

internal static class CommandList
{
    public const string Cancel = "/cancel";
    public const string NewTeam = "/new_team";
    public const string LeaveTeam = "/leave_team";
    public const string RemoveTeam = "/remove_team";
    
    public const string MoveToSp = "/move_to_sp";
    public const string MoveToTShirts = "/move_to_tshirts";
    public const string AddStory = "/add";

    public const string ChangeToRoundRobin = "/change_to_round_robin";
    public const string ChangeToRandom = "/change_to_random";
    public const string NeedReview = "/need_review";
    
    public const string AddLocation = "/location";

    public const string AddPollAnswer = "/poll_answer?pollId={0}&option={1}";
}