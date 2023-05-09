namespace Inc.TeamAssistant.Appraiser.Model;

public interface ICookieService
{
    bool IsServerRender { get; }
    
    Task<string?> GetValue(string name);

    Task SetValue(string name, string value, int lifetimeInDays);
}