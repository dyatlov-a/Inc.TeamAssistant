namespace Inc.TeamAssistant.WebUI.Features.Survey;

public interface ISurveyEventProvider
{
    IDisposable OnFacilitatorChanged(Func<long, Task> changed);
}