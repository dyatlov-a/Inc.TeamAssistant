using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroEventSender
{
    Task RetroItemChanged(RetroItemDto item, bool excludedOwner = false);

    Task RetroItemRemoved(Guid teamId, Guid itemId);
    
    Task RetroSessionChanged(RetroSessionDto session);
    
    Task VotesChanged(Guid teamId, long personId, int votesCount);

    Task PersonsChanged(Guid teamId, IReadOnlyCollection<Person> persons);

    Task ActionItemChanged(Guid teamId, ActionItemDto item);
}