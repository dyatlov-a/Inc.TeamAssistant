using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IMessageProvider
{
    Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> Get();
}