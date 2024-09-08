namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public interface IOnGroupHandlerBuilder
{
    IDisposable OnStoryChanged(Func<Task> changed);

    IDisposable OnStoryAccepted(Func<string, Task> accepted);
}