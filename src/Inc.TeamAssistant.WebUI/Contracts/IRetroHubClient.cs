using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroHubClient
{
    Task RetroItemChanged(RetroItemDto item);
    
    Task RetroItemRemoved(RetroItemDto item);

    Task RetroSessionChanged(RetroSessionDto session);
    
    Task VotesChanged(long personId, int votesCount);
    
    Task PersonsChanged(IReadOnlyCollection<Person> persons);
}