using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IDataEditor
{
    Task<ServiceResult<string?>> Get(string key, CancellationToken token = default);
    
    Task Attach(string key, string data, CancellationToken token = default);

    Task Detach(string key, CancellationToken token = default);
}