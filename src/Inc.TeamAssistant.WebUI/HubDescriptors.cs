namespace Inc.TeamAssistant.WebUI;

public static class HubDescriptors
{
    public static class AssessmentSessionHub
    {
        public const string Endpoint = "/assessment-session-events";
        public const string JoinToAssessmentSessionMethod = "JoinToAssessmentSession";
        public const string RemoveFromAssessmentSessionMethod = "RemoveFromAssessmentSession";
    }
    
    public static class RetroHub
    {
        public const string Endpoint = "/retro-events";
        public const string JoinRetroMethod = "JoinRetro";
        public const string LeaveRetroMethod = "LeaveRetro";
        public const string CreateRetroItemMethod = "CreateRetroItem";
        public const string UpdateRetroItemMethod = "UpdateRetroItem";
        public const string RemoveRetroItemMethod = "RemoveRetroItem";
        public const string SetVotesMethod = "SetVotes";
        public const string SetRetroStateMethod = "SetRetroState";
        public const string MoveItemMethod = "MoveItem";
        public const string ChangeActionItemMethod = "ChangeActionItem";
        public const string RemoveActionItemMethod = "RemoveActionItem";
        public const string ChangeTimerMethod = "ChangeTimer";
    }
}