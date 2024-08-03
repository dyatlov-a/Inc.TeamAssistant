namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IPersonPhotosService
{
    Task<byte[]?> GetPersonPhoto(long personId, CancellationToken token);
}