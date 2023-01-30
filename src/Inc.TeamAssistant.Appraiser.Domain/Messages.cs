using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain;

internal static class Messages
{
    public static readonly MessageId NoRightsToMakeOperation = new(nameof(NoRightsToMakeOperation));
    public static readonly MessageId ModeratorCannotDisconnectedFromSession = new(nameof(ModeratorCannotDisconnectedFromSession));
    public static readonly MessageId MissingTaskForEvaluate = new(nameof(MissingTaskForEvaluate));
    public static readonly MessageId NotValidState = new(nameof(NotValidState));
    public static readonly MessageId AppraiserConnectWithError = new(nameof(AppraiserConnectWithError));
    public static readonly MessageId EstimateRejected = new(nameof(EstimateRejected));
}