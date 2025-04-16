using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Application;

internal static class Messages
{
    public static readonly MessageId Appraiser_EndEstimate = new(nameof(Appraiser_EndEstimate));
    public static readonly MessageId Appraiser_NeedEstimate = new(nameof(Appraiser_NeedEstimate));
    public static readonly MessageId Appraiser_MeanEstimate = new(nameof(Appraiser_MeanEstimate));
    public static readonly MessageId Appraiser_MedianEstimate = new(nameof(Appraiser_MedianEstimate));
    public static readonly MessageId Appraiser_AcceptedEstimate = new(nameof(Appraiser_AcceptedEstimate));
    public static readonly MessageId Appraiser_Accept = new(nameof(Appraiser_Accept));
    public static readonly MessageId Appraiser_Finish = new(nameof(Appraiser_Finish));
    public static readonly MessageId Appraiser_Revote = new(nameof(Appraiser_Revote));
    public static readonly MessageId Appraiser_MissingTaskForEvaluate = new(nameof(Appraiser_MissingTaskForEvaluate));
    public static readonly MessageId Appraiser_NumberOfRounds = new(nameof(Appraiser_NumberOfRounds));
    public static readonly MessageId Appraiser_MultipleLinkError = new(nameof(Appraiser_MultipleLinkError));
    public static readonly MessageId Appraiser_LinkLengthError = new(nameof(Appraiser_LinkLengthError));
}