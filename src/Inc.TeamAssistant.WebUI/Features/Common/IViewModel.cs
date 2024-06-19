namespace Inc.TeamAssistant.WebUI.Features.Common;

public interface IViewModel<out TViewModel>
{
    static virtual string PersistentKey { get; } = typeof(TViewModel).FullName!;
    
    static abstract TViewModel Empty { get; }
}