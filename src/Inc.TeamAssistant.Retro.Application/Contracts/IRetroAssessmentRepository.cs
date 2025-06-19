using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroAssessmentRepository
{
    Task<RetroAssessment?> Find(Guid sessionId, long personId, CancellationToken token);
    
    Task Upsert(RetroAssessment assessment, CancellationToken token);
}