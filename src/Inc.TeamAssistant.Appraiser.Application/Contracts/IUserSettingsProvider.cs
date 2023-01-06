using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IUserSettingsProvider
{
    Task<string> GetUserName(ParticipantId userId, CancellationToken cancellationToken);

    Task SetUserName(ParticipantId userId, string userName, CancellationToken cancellationToken);
}