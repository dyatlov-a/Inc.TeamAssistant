using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Model;

public interface IMessageProvider
{
    Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> Get();
}