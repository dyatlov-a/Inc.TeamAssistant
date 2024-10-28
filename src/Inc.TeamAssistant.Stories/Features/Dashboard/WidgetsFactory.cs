using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

namespace Inc.TeamAssistant.Stories.Features.Dashboard;

internal static class WidgetsFactory
{
    public static IReadOnlyCollection<WidgetDto> CreateAllWidgets() => CreateWidgets(
        hasReviewer: true,
        hasAppraiser: true,
        hasRandomCoffee: true,
        hasCheckIn: true);
    
    public static IReadOnlyCollection<WidgetDto> CreateReviewerWidgets() => CreateWidgets(hasReviewer: true);
    public static IReadOnlyCollection<WidgetDto> CreateAppraiserWidgets() => CreateWidgets(hasAppraiser: true);
    public static IReadOnlyCollection<WidgetDto> CreateRandomCoffeeWidgets() => CreateWidgets(hasRandomCoffee: true);
    public static IReadOnlyCollection<WidgetDto> CreateCheckInWidgets() => CreateWidgets(hasCheckIn: true);
    
    private static IReadOnlyCollection<WidgetDto> CreateWidgets(
        bool hasReviewer = false,
        bool hasAppraiser = false,
        bool hasRandomCoffee = false,
        bool hasCheckIn = false)
    {
        return
        [
            new(
                Type: "TeammatesWidget",
                Feature: string.Empty,
                Position: 1,
                CanEnabled: true,
                IsEnabled: true),
            new(
                Type: "ReviewTotalStatsWidget",
                Feature: "Reviewer",
                Position: 2,
                CanEnabled: hasReviewer,
                IsEnabled: hasReviewer),
            new(
                Type: "ReviewHistoryWidget",
                Feature: "Reviewer",
                Position: 3,
                CanEnabled: hasReviewer,
                IsEnabled: hasReviewer),
            new(
                Type: "ReviewAverageStatsWidget",
                Feature: "Reviewer",
                Position: 4,
                CanEnabled: hasReviewer,
                IsEnabled: hasReviewer),
            new(
                Type: "AppraiserHistoryWidget",
                Feature: "Appraiser",
                Position: 5,
                CanEnabled: hasAppraiser,
                IsEnabled: hasAppraiser),
            new(
                Type: "AppraiserIntegrationWidget",
                Feature: "Appraiser",
                Position: 6,
                CanEnabled: hasAppraiser,
                IsEnabled: hasAppraiser),
            new(
                Type: "RandomCoffeeHistoryWidget",
                Feature: "RandomCoffee",
                Position: 7,
                CanEnabled: hasRandomCoffee,
                IsEnabled: hasRandomCoffee),
            new(
                Type: "MapWidget",
                Feature: "CheckIn",
                Position: 8,
                CanEnabled: hasCheckIn,
                IsEnabled: hasCheckIn)
        ];
    }
}