namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IPersonPhotosService
{
    Task<MemoryStream?> GetPersonPhoto(long personId, CancellationToken token);
}