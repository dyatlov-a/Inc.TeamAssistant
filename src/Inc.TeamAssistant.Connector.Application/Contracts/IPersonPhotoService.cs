namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IPersonPhotoService
{
    Task<byte[]?> GetPersonPhoto(long personId, CancellationToken token);
}