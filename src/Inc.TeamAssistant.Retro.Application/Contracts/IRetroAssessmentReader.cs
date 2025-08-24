namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroAssessmentReader
{
    Task<IReadOnlyCollection<int>> Read(Guid retroSessionId, CancellationToken token);
    
    Task<(Guid RoomId, int? Value)> Read(Guid retroSessionId, long personId, CancellationToken token);
}