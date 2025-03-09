namespace Inc.TeamAssistant.Primitives.Extensions;

public static class GuidExtensions
{
    public static string ToLinkSegment(this Guid value) => value.ToString("N");
}