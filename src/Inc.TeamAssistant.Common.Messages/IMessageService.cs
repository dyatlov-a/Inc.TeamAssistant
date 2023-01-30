using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Common.Messages;

public interface IMessageService
{
    Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> GetAll(
        CancellationToken cancellationToken = default);
}