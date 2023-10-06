using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class UserSettingsProviderFailTolerance : IUserSettingsProvider
{
    private readonly IUserSettingsProvider _userSettingsProvider;
    private readonly ILogger<UserSettingsProviderFailTolerance> _logger;
    private readonly string _anonymousUser;

    public UserSettingsProviderFailTolerance(
        IUserSettingsProvider userSettingsProvider,
        ILogger<UserSettingsProviderFailTolerance> logger,
        string anonymousUser)
    {
        if (string.IsNullOrWhiteSpace(anonymousUser))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(anonymousUser));

        _userSettingsProvider = userSettingsProvider ?? throw new ArgumentNullException(nameof(userSettingsProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _anonymousUser = anonymousUser;
    }

    public async Task<string> GetUserName(ParticipantId userId, CancellationToken cancellationToken)
    {
        if (userId is null)
            throw new ArgumentNullException(nameof(userId));

        try
        {
            return await _userSettingsProvider.GetUserName(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Problems reading from database");
        }

        return _anonymousUser;
    }

    public async Task SetUserName(ParticipantId userId, string userName, CancellationToken cancellationToken)
    {
        if (userId is null)
            throw new ArgumentNullException(nameof(userId));

        try
        {
            await _userSettingsProvider.SetUserName(userId, userName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Problems writing to database");
        }
    }
}