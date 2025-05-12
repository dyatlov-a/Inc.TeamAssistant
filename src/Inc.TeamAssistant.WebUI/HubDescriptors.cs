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
        public const string JoinToRetroMethod = "JoinToRetro";
        public const string RemoveFromRetroMethod = "RemoveFromRetro";
        public const string CreateRetroItemMethod = "CreateRetroItem";
        public const string UpdateRetroItemMethod = "UpdateRetroItem";
        public const string RemoveRetroItemMethod = "RemoveRetroItem";
    }
}