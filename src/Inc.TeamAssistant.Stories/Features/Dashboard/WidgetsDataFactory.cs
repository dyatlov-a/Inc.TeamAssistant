using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using Inc.TeamAssistant.WebUI.Components;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;
using Inc.TeamAssistant.WebUI.Features.Dashboard.CheckIn;
using Inc.TeamAssistant.WebUI.Features.Dashboard.RandomCoffee;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Teams;
using Inc.TeamAssistant.WebUI.Routing;

namespace Inc.TeamAssistant.Stories.Features.Dashboard;

internal sealed class WidgetsDataFactory
{
    private readonly IDateSelectorFactory _dateFactory;

    public WidgetsDataFactory(IDateSelectorFactory dateFactory)
    {
        _dateFactory = dateFactory ?? throw new ArgumentNullException(nameof(dateFactory));
    }

    public Dictionary<string, object?> Create(Type type, Guid teamId)
    {
        ArgumentNullException.ThrowIfNull(type);

        var weeks = _dateFactory.CreateWeeks();
        var months = _dateFactory.CreateMonths();
        
        if (type == typeof(TeammatesWidget))
            return CreateDataForTeammatesWidget(teamId);
        if (type == typeof(ReviewAverageStatsWidget))
            return CreateDataForReviewAverageStatsWidget(teamId, weeks);
        if (type == typeof(ReviewHistoryWidget))
            return CreateDataForReviewHistoryWidget(teamId, weeks);
        if (type == typeof(ReviewTotalStatsWidget))
            return CreateDataForReviewTotalStatsWidget(teamId, weeks);
        if (type == typeof(AppraiserHistoryWidget))
            return CreateDataForAppraiserHistoryWidget(teamId, months);
        if (type == typeof(AppraiserIntegrationWidget))
            return CreateDataForAppraiserIntegrationWidget(teamId);
        if (type == typeof(RandomCoffeeHistoryWidget))
            return CreateDataForRandomCoffeeHistoryWidget();
        if (type == typeof(MapWidget))
            return CreateDataForMapWidget();
        
        throw new NotSupportedException($"{type.Name} is not supported.");
    }

    private Dictionary<string, object?> CreateDataForTeammatesWidget(Guid teamId)
    {
        return new Dictionary<string, object?>
        {
            ["TeamId"] = teamId,
            ["State"] = LoadingState.Done(),
            ["Item"] = new GetTeammatesResult(
                HasManagerAccess: true,
                Teammates: 
                [
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 1,
                        Name: "Aleksandr",
                        UserName: "adiatlov",
                        LeaveUntil: null,
                        CanFinalize: true),
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 2,
                        Name: "Alex",
                        UserName: null,
                        LeaveUntil: null,
                        CanFinalize: false),
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 3,
                        Name: "Ivan",
                        UserName: null,
                        LeaveUntil: null,
                        CanFinalize: false),
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 4,
                        Name: "Petr",
                        UserName: null,
                        LeaveUntil: null,
                        CanFinalize: false),
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 5,
                        Name: "Samson",
                        UserName: null,
                        LeaveUntil: null,
                        CanFinalize: false),
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 6,
                        Name: "Konstantin",
                        UserName: null,
                        LeaveUntil: null,
                        CanFinalize: false),
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 7,
                        Name: "Pavel",
                        UserName: null,
                        LeaveUntil: null,
                        CanFinalize: false),
                    new TeammateDto(
                        TeamId: teamId,
                        PersonId: 8,
                        Name: "Andrey",
                        UserName: null,
                        LeaveUntil: null,
                        CanFinalize: false)
                ]),
            ["Retry"] = () => Task.CompletedTask
        };
    }

    private Dictionary<string, object?> CreateDataForReviewAverageStatsWidget(
        Guid teamId,
        IReadOnlyDictionary<string, DateOnly> weeks)
    {
        ArgumentNullException.ThrowIfNull(weeks);
        
        return new Dictionary<string, object?>
        {
            ["TeamId"] = teamId,
            ["Date"] = weeks.First().Value,
            ["DateItems"] = weeks,
            ["State"] = LoadingState.Done(),
            ["Items"] = new[]
            {
                new ReviewAverageStatsDto(
                    Created: new DateOnly(2024, 10, 28),
                    FirstTouch: TimeSpan.FromMinutes(8),
                    Review: TimeSpan.FromMinutes(21),
                    Correction: TimeSpan.FromMinutes(4)),
                new ReviewAverageStatsDto(
                    Created: new DateOnly(2024, 10, 29),
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Review: TimeSpan.FromMinutes(30),
                    Correction: TimeSpan.FromMinutes(10)),
                new ReviewAverageStatsDto(
                    Created: new DateOnly(2024, 10, 30),
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Review: TimeSpan.FromMinutes(30),
                    Correction: TimeSpan.FromMinutes(10)),
                new ReviewAverageStatsDto(
                    Created: new DateOnly(2024, 11, 1),
                    FirstTouch: TimeSpan.FromMinutes(12),
                    Review: TimeSpan.FromMinutes(40),
                    Correction: TimeSpan.FromMinutes(14)),
                new ReviewAverageStatsDto(
                    Created: new DateOnly(2024, 11, 2),
                    FirstTouch: TimeSpan.FromMinutes(8),
                    Review: TimeSpan.FromMinutes(21),
                    Correction: TimeSpan.FromMinutes(4))
            },
            ["Retry"] = () => Task.CompletedTask
        };
    }

    private Dictionary<string, object?> CreateDataForReviewHistoryWidget(
        Guid teamId,
        IReadOnlyDictionary<string, DateOnly> weeks)
    {
        ArgumentNullException.ThrowIfNull(weeks);
        
        return new Dictionary<string, object?>
        {
            ["TeamId"] = teamId,
            ["Date"] = weeks.First().Value,
            ["DateItems"] = weeks,
            ["State"] = LoadingState.Done(),
            ["Items"] = new[]
            {
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "AcceptWithComments",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    ["It is necessary to start a task for refactoring"]),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    []),
                new TaskForReviewDto(
                    Id: Guid.NewGuid(),
                    Created: DateTimeOffset.UtcNow,
                    State: "Accept",
                    Description: "Description",
                    FirstTouch: TimeSpan.FromMinutes(10),
                    Correction: TimeSpan.FromMinutes(20),
                    Review: TimeSpan.FromMinutes(40),
                    Iterations: 2,
                    ReviewerId: 1,
                    ReviewerName: "Aleksandr",
                    ReviewerUserName: "adiatlov",
                    OwnerId: 2,
                    OwnerName: "Alex",
                    OwnerUserName: null,
                    HasConcreteReviewer: false,
                    HasReassign: false,
                    [])
            },
            ["Retry"] = () => Task.CompletedTask
        };
    }

    private Dictionary<string, object?> CreateDataForReviewTotalStatsWidget(
        Guid teamId,
        IReadOnlyDictionary<string, DateOnly> weeks)
    {
        ArgumentNullException.ThrowIfNull(weeks);
        
        return new Dictionary<string, object?>
        {
            ["TeamId"] = teamId,
            ["Date"] = weeks.First().Value,
            ["DateItems"] = weeks,
            ["State"] = LoadingState.Done(),
            ["FormModel"] = new ReviewTotalStatsWidgetFormModel().Apply(new GetHistoryByTeamResult(
                Review:
                [
                    new HistoryByTeamItemDto(PersonName: "Aleksandr (adiatlov)", Count: 10),
                    new HistoryByTeamItemDto(PersonName: "Alex", Count: 6),
                    new HistoryByTeamItemDto(PersonName: "Ivan", Count: 4),
                    new HistoryByTeamItemDto(PersonName: "Petr", Count: 2)
                ],
                Requests:
                [
                    new HistoryByTeamItemDto(PersonName: "Aleksandr (adiatlov)", Count: 6),
                    new HistoryByTeamItemDto(PersonName: "Alex", Count: 10),
                    new HistoryByTeamItemDto(PersonName: "Ivan", Count: 2),
                    new HistoryByTeamItemDto(PersonName: "Petr", Count: 4)
                ]), weeks.First().Value),
            ["Retry"] = () => Task.CompletedTask
        };
    }

    private Dictionary<string, object?> CreateDataForAppraiserHistoryWidget(
        Guid teamId,
        IReadOnlyDictionary<string, DateOnly> months)
    {
        ArgumentNullException.ThrowIfNull(months);
        
        return new Dictionary<string, object?>
        {
            ["TeamId"] = teamId,
            ["Date"] = months.First().Value,
            ["DateItems"] = months,
            ["State"] = LoadingState.Done(),
            ["Items"] = new[]
            {
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 11, 18),
                    StoriesCount: 10,
                    AssessmentSum: 80),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 11, 5),
                    StoriesCount: 10,
                    AssessmentSum: 50),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 10, 21),
                    StoriesCount: 3,
                    AssessmentSum: 63),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 10, 7),
                    StoriesCount: 10,
                    AssessmentSum: 80),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 9, 23),
                    StoriesCount: 10,
                    AssessmentSum: 50),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 9, 9),
                    StoriesCount: 10,
                    AssessmentSum: 61),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 8, 26),
                    StoriesCount: 5,
                    AssessmentSum: 25),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 8, 12),
                    StoriesCount: 3,
                    AssessmentSum: 6),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 7, 29),
                    StoriesCount: 4,
                    AssessmentSum: 7),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 7, 15),
                    StoriesCount: 4,
                    AssessmentSum: 7),
                new AssessmentHistoryDto(
                    AssessmentDate: new DateOnly(2024, 7, 1),
                    StoriesCount: 4,
                    AssessmentSum: 7)
            },
            ["LinkFactory"] = (string _) => new NavRoute(LanguageId: null, RouteSegment: "#"),
            ["Retry"] = () => Task.CompletedTask
        };
    }

    private Dictionary<string, object?> CreateDataForAppraiserIntegrationWidget(Guid teamId)
    {
        return new Dictionary<string, object?>
        {
            ["TeamId"] = teamId,
            ["State"] = LoadingState.Done(),
            ["FormModel"] = new AppraiserIntegrationFromModel().Apply(new GetIntegrationPropertiesResult(
                Properties: new IntegrationProperties(
                    AccessToken: Guid.NewGuid().ToLinkSegment(),
                    ProjectKey: "test_project",
                    ScrumMasterId: 1),
                HasManagerAccess: true,
                Teammates:
                [
                    new Person(Id: 1, Name: "Aleksandr", Username: "adiatlov"),
                    new Person(Id: 2, Name: "Alex", Username: null)
                ])),
            ["Retry"] = () => Task.CompletedTask
        };
    }

    private Dictionary<string, object?> CreateDataForRandomCoffeeHistoryWidget()
    {
        return new Dictionary<string, object?>
        {
            ["State"] = LoadingState.Done(),
            ["FormModel"] = new RandomCoffeeHistoryWidgetFormModel().Apply(
                new RandomCoffeeHistoryWidgetFormModel.Parameters(
                    new GetChatsResult([
                        new ChatDto(Id: 1, Name: "Chat 1"),
                        new ChatDto(Id: 2, Name: "Chat 2")
                    ]),
                    ChatId: 1,
                    History: new GetHistoryResult(
                    [
                        new RandomCoffeeHistoryDto(
                            Created: new DateOnly(2024, 10, 28),
                            Pairs:
                            [
                                new PairDto(
                                    FirstName: "Aleksandr",
                                    FirstUserName: "adiatlov",
                                    SecondName: "Alex",
                                    SecondUserName: null),
                                new PairDto(
                                    FirstName: "Ivan",
                                    FirstUserName: null,
                                    SecondName: "Petr",
                                    SecondUserName: null),
                                new PairDto(
                                    FirstName: "Samson",
                                    FirstUserName: null,
                                    SecondName: "Konstantin",
                                    SecondUserName: null),
                                new PairDto(
                                    FirstName: "Pavel",
                                    FirstUserName: null,
                                    SecondName: "Andrey",
                                    SecondUserName: null)
                            ],
                            ExcludedPersonName: null,
                            ExcludedPersonUserName: null)
                    ])))
        };
    }

    private Dictionary<string, object?> CreateDataForMapWidget()
    {
        return new Dictionary<string, object?>
        {
            ["State"] = LoadingState.Done(),
            ["FormModel"] = new MapWidgetFormModel().Apply(new GetMapsResult(
            [
                new MapDto(Id: Guid.NewGuid(), Name: "Chat 1"),
                new MapDto(Id: Guid.NewGuid(), Name: "Chat 2")
            ])),
            ["Retry"] = () => Task.CompletedTask,
            ["LinkFactory"] = (string _) => "/imgs/map.png"
        };
    }
}