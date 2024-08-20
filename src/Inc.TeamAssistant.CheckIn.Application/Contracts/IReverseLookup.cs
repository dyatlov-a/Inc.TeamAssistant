namespace Inc.TeamAssistant.CheckIn.Application.Contracts;

public interface IReverseLookup
{
    Region? Lookup(float lat, float lng, params RegionType[] types);
}