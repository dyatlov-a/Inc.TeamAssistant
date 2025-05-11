namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public interface IAssessmentSessionProvider
{
    IDisposable OnStoryChanged(Func<Task> changed);

    IDisposable OnStoryAccepted(Func<string, Task> accepted);
}