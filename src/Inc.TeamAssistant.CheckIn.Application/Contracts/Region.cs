namespace Inc.TeamAssistant.CheckIn.Application.Contracts;

public sealed record Region(
    string Name,
    string Code,
    RegionType Type,
    float[][] Polygon);