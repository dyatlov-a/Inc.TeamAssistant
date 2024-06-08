using Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

namespace Inc.TeamAssistant.WebUI;

internal static class Messages
{
    public static readonly string GUI_Authorizing = nameof(GUI_Authorizing);
    public static readonly string GUI_Error401 = nameof(GUI_Error401);
    public static readonly string GUI_Unauthorized = nameof(GUI_Unauthorized);
    public static readonly string GUI_Error404 = nameof(GUI_Error404);
    public static readonly string GUI_PageNotFound = nameof(GUI_PageNotFound);

    public static readonly string GUI_TaskAssess = nameof(GUI_TaskAssess);
    public static readonly string GUI_MeanRating = nameof(GUI_MeanRating);
    public static readonly string GUI_MedianRating = nameof(GUI_MedianRating);
    public static readonly string GUI_AssessmentSessionAbout = nameof(GUI_AssessmentSessionAbout);
    public static readonly string GUI_AssessmentSessionConnect = nameof(GUI_AssessmentSessionConnect);
    public static readonly string GUI_AssessmentSessionHasNotTasks = nameof(GUI_AssessmentSessionHasNotTasks);

    public static readonly string GUI_AcceptCookieButton = nameof(GUI_AcceptCookieButton);
    public static readonly string GUI_AcceptCookieText = nameof(GUI_AcceptCookieText);

    public static readonly string GUI_History = nameof(GUI_History);
    public static readonly string GUI_Tasks = nameof(GUI_Tasks);
    public static readonly string GUI_StoryList = nameof(GUI_StoryList);
    public static readonly string GUI_Back = nameof(GUI_Back);
    
    public static readonly string GUI_RequestDemo = nameof(GUI_RequestDemo);
    public static readonly string GUI_RequestDemoBody = nameof(GUI_RequestDemoBody);
    public static readonly string GUI_MainHeader = nameof(GUI_MainHeader);
    public static readonly string GUI_MainSubHeader = nameof(GUI_MainSubHeader);
    public static readonly string GUI_ToolAppraiser = nameof(GUI_ToolAppraiser);
    public static readonly string GUI_ToolReviewer = nameof(GUI_ToolReviewer);
    public static readonly string GUI_ToolRandomCoffee = nameof(GUI_ToolRandomCoffee);
    public static readonly string GUI_CreateBotLink = nameof(GUI_CreateBotLink);
    public static readonly string GUI_VideoNotSupported = nameof(GUI_VideoNotSupported);

    public static readonly string Navigation_Logout = nameof(Navigation_Logout);
    public static readonly string Navigation_LoginAsSuperuser = nameof(Navigation_LoginAsSuperuser);

    public static readonly string Constructor_NewBot = nameof(Constructor_NewBot);
    public static readonly string Constructor_AddBot = nameof(Constructor_AddBot);
    public static readonly string Constructor_Edit = nameof(Constructor_Edit);
    public static readonly string Constructor_Remove = nameof(Constructor_Remove);
    public static readonly string Constructor_Title = nameof(Constructor_Title);
    public static readonly string Constructor_SelectBotText = nameof(Constructor_SelectBotText);
    public static readonly string Constructor_LoginTelegramText = nameof(Constructor_LoginTelegramText);
    public static readonly string Constructor_RemoveConfirmationText = nameof(Constructor_RemoveConfirmationText);
    public static readonly string Constructor_FormSectionTokenTitle = nameof(Constructor_FormSectionTokenTitle);
    public static readonly string Constructor_FormSectionTokenHelpTemplate = nameof(Constructor_FormSectionTokenHelpTemplate);
    public static readonly string Constructor_FormSectionTokenFieldTokenLabel = nameof(Constructor_FormSectionTokenFieldTokenLabel);
    public static readonly string Constructor_FormSectionTokenFieldUserNameLabel = nameof(Constructor_FormSectionTokenFieldUserNameLabel);
    public static readonly string Constructor_MoveNextTitle = nameof(Constructor_MoveNextTitle);
    public static readonly string Constructor_FormSectionFeaturesTitle = nameof(Constructor_FormSectionFeaturesTitle);
    public static readonly string Constructor_FormSectionFeaturesHelp = nameof(Constructor_FormSectionFeaturesHelp);
    public static readonly string Constructor_FeatureAppraiserName = nameof(Constructor_FeatureAppraiserName);
    public static readonly string Constructor_FeatureReviewerName = nameof(Constructor_FeatureReviewerName);
    public static readonly string Constructor_FeatureRandomCoffeeName = nameof(Constructor_FeatureRandomCoffeeName);
    public static readonly string Constructor_FeatureCheckInName = nameof(Constructor_FeatureCheckInName);
    public static readonly string Constructor_FeatureAdd = nameof(Constructor_FeatureAdd);
    public static readonly string Constructor_FeatureRemove = nameof(Constructor_FeatureRemove);
    public static readonly string Constructor_FormSectionFeaturesAvailableEmptyText = nameof(Constructor_FormSectionFeaturesAvailableEmptyText);
    public static readonly string Constructor_FormSectionFeaturesSelectedEmptyText = nameof(Constructor_FormSectionFeaturesSelectedEmptyText);
    public static readonly string Constructor_FormSectionSetSettingsScrumDescription = nameof(Constructor_FormSectionSetSettingsScrumDescription);
    public static readonly string Constructor_FormSectionSetSettingsKanbanDescription = nameof(Constructor_FormSectionSetSettingsKanbanDescription);
    public static readonly string Constructor_FormSectionSetSettingsRoundRobinDescription = nameof(Constructor_FormSectionSetSettingsRoundRobinDescription);
    public static readonly string Constructor_FormSectionSetSettingsRandomDescription = nameof(Constructor_FormSectionSetSettingsRandomDescription);
    public static readonly string Constructor_FormSectionSetSettingsAppraiserHeader = nameof(Constructor_FormSectionSetSettingsAppraiserHeader);
    public static readonly string Constructor_FormSectionSetSettingsAppraiserHelp = nameof(Constructor_FormSectionSetSettingsAppraiserHelp);
    public static readonly string Constructor_FormSectionSetSettingsStoryTypeFieldLabel = nameof(Constructor_FormSectionSetSettingsStoryTypeFieldLabel);
    public static readonly string Constructor_FormSectionSetSettingsReviewerHeader = nameof(Constructor_FormSectionSetSettingsReviewerHeader);
    public static readonly string Constructor_FormSectionSetSettingsReviewerHelp = nameof(Constructor_FormSectionSetSettingsReviewerHelp);
    public static readonly string Constructor_FormSectionSetSettingsNextReviewerStrategyFieldLabel = nameof(Constructor_FormSectionSetSettingsNextReviewerStrategyFieldLabel);
    public static readonly string Constructor_FormSectionTokenCheckHelp = nameof(Constructor_FormSectionTokenCheckHelp);
    public static readonly string Constructor_FormSectionFeaturesCheckHelp = nameof(Constructor_FormSectionFeaturesCheckHelp);
    public static readonly string Constructor_ButtonCreateText = nameof(Constructor_ButtonCreateText);
    public static readonly string Constructor_ButtonUpdateText = nameof(Constructor_ButtonUpdateText);
    public static readonly string Constructor_BooleanTrueText = nameof(Constructor_BooleanTrueText);
    public static readonly string Constructor_BooleanFalseText = nameof(Constructor_BooleanFalseText);

    public static readonly string Dashboard_MoveToStats = nameof(Dashboard_MoveToStats);
    public static readonly string Dashboard_Title = nameof(Dashboard_Title);
    public static readonly string Dashboard_SelectTeam = nameof(Dashboard_SelectTeam);
    public static readonly string Dashboard_CreateBot = nameof(Dashboard_CreateBot);
    public static readonly string Dashboard_SelectTeamTitle = nameof(Dashboard_SelectTeamTitle);
    public static readonly string Dashboard_TeamField = nameof(Dashboard_TeamField);
    public static readonly string Dashboard_BotField = nameof(Dashboard_BotField);
    public static readonly string Dashboard_ExcludeFromTeam = nameof(Dashboard_ExcludeFromTeam);

    public static readonly string ConfirmDialog_Yes = nameof(ConfirmDialog_Yes);
    public static readonly string ConfirmDialog_No = nameof(ConfirmDialog_No);

    public static readonly string Validation_TokenInvalid = nameof(Validation_TokenInvalid);
    
    public static readonly string OgTitle = nameof(OgTitle);
    public static readonly string OgDescription = nameof(OgDescription);
    
    public static readonly string CheckIn_PageTitle = nameof(CheckIn_PageTitle);
    public static readonly string CheckIn_DefaultLayerTitle = nameof(CheckIn_DefaultLayerTitle);
    public static readonly string CheckIn_OgTitle = nameof(CheckIn_OgTitle);
    public static readonly string CheckIn_OgDescription = nameof(CheckIn_OgDescription);
    public static readonly string CheckIn_RouteShow = nameof(CheckIn_RouteShow);
    public static readonly string CheckIn_RouteHide = nameof(CheckIn_RouteHide);
    
    public static readonly string MetaTitle = nameof(MetaTitle);
    public static readonly string MetaDescription = nameof(MetaDescription);
    public static readonly string MetaKeywords = nameof(MetaKeywords);
    public static readonly string MetaAuthor = nameof(MetaAuthor);
    
    public static string GetStageTitle(Stage stage) => $"Constructor_Stage{stage}";

    public static IReadOnlyDictionary<string, string> BuildFeatureNames(IReadOnlyDictionary<string, string> resources)
    {
        ArgumentNullException.ThrowIfNull(resources);
        
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["Appraiser"] = resources[Constructor_FeatureAppraiserName],
            ["Reviewer"] = resources[Constructor_FeatureReviewerName],
            ["RandomCoffee"] = resources[Constructor_FeatureRandomCoffeeName],
            ["CheckIn"] = resources[Constructor_FeatureCheckInName]
        };
    }
}