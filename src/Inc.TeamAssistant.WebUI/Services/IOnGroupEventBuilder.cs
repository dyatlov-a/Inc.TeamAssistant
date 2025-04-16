namespace Inc.TeamAssistant.WebUI.Services;

public interface IOnGroupEventBuilder
{
    IDisposable OnStoryChanged(Func<Task> changed);

    IDisposable OnStoryAccepted(Func<string, Task> accepted);
}