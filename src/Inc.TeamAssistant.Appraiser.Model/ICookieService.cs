namespace Inc.TeamAssistant.Appraiser.Model;

public interface ICookieService
{
    Task<string?> GetValue(string name);

    Task SetValue(string name, string value, int lifetimeInDays);
}