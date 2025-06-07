namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IFacilitatorProvider
{
    long? Get(Guid teamId);
    
    void Set(Guid teamId, long personId);
}