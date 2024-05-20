using Inc.TeamAssistant.Primitives.Bots;
using Telegram.Bot.Extensions.LoginWidget;

namespace Inc.TeamAssistant.Gateway.Services.Core;

public sealed class TelegramAuthService
{
    private readonly AuthOptions _options;
    private readonly IBotAccessor _botAccessor;

    public TelegramAuthService(AuthOptions options, IBotAccessor botAccessor)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task<bool> CanLogin(
        long id,
        string firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        string authDate,
        string hash)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(authDate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(authDate));
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(hash));

        var fields = CreateFieldSet(id, firstName, lastName, username, photoUrl, authDate, hash);
        var token = await _botAccessor.GetToken(_options.BotId);
        var loginWidget = new LoginWidget(token);
        var authState = loginWidget.CheckAuthorization(fields);
        
        return authState == Authorization.Valid;
    }

    private Dictionary<string, string> CreateFieldSet(
        long id,
        string firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        string authDate,
        string hash)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(authDate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(authDate));
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(hash));
        
        var fields = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
            ["first_name"] = firstName
        };
        
        if (lastName is not null)
            fields.Add("last_name", lastName);
        if (lastName is not null)
            fields.Add("username", username!);
        if (lastName is not null)
            fields.Add("photo_url", photoUrl!);
        
        fields.Add("auth_date", authDate);
        fields.Add("hash", hash);

        return fields;
    }
}