namespace Inc.TeamAssistant.WebUI.Features.Common;

public interface IViewModel<out TViewModel>
{
    static abstract TViewModel Empty { get; }
}