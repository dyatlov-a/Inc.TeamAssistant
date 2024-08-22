namespace Inc.TeamAssistant.CheckIn.Application.Contracts;

public interface IGeoService
{
    Region? FindCountry(double lat, double lng, params RegionType[] types);
    
    TimeZoneInfo GetTimeZone(double lat, double lng);
}