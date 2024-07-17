using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Telegram.Bot.Extensions.LoginWidget;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

public sealed class TelegramAuthService
{
    private readonly AuthOptions _options;
    private readonly IBotAccessor _botAccessor;
    private readonly ILogger<TelegramAuthService> _logger;

    public TelegramAuthService(AuthOptions options, IBotAccessor botAccessor, ILogger<TelegramAuthService> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        var canLogin = authState == Authorization.Valid;

        if (!canLogin)
        {
            var person = new Person(id, firstName, username);
            
            _logger.LogError(
                "User {Id} {UserName} is not valid {AuthState}",
                person.Id,
                person.DisplayName,
                authState);
        }

        return canLogin;
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
        if (username is not null)
            fields.Add("username", username);
        if (photoUrl is not null)
            fields.Add("photo_url", photoUrl);
        
        fields.Add("auth_date", authDate);
        fields.Add("hash", hash);

        return fields;
    }
}