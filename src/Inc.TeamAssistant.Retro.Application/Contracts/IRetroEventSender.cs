using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroEventSender
{
    Task RetroItemChanged(RetroItemDto item);

    Task RetroItemRemoved(RetroItemDto item);
    
    Task RetroSessionChanged(RetroSessionDto session);

    Task PersonsChanged(Guid teamId, IReadOnlyCollection<Person> persons);
}