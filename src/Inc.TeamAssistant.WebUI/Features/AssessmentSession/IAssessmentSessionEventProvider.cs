namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public interface IAssessmentSessionEventProvider
{
    IDisposable OnStoryChanged(Func<Task> changed);

    IDisposable OnStoryAccepted(Func<string, Task> accepted);
}