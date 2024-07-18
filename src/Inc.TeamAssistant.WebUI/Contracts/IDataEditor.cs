using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IDataEditor
{
    Task<ServiceResult<string?>> Get(Guid dataId, CancellationToken token = default);
    
    Task Attach(Guid dataId, string data, CancellationToken token = default);
}