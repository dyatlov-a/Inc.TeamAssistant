namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroAssessmentReader
{
    Task<(Guid RoomId, int? Value)> Read(Guid retroSessionId, long personId, CancellationToken token);
}