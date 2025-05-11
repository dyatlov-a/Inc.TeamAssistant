using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroEventSender
{
    Task RetroItemChanged(RetroItemDto item);

    Task RetroItemRemoved(RetroItemDto item);
}