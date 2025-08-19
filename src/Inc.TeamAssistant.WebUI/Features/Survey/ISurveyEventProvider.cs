using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.WebUI.Features.Survey;

public interface ISurveyEventProvider
{
    IDisposable OnSurveyStarted(Func<Task> changed);
    
    IDisposable OnFacilitatorChanged(Func<long, Task> changed);

    IDisposable OnPersonsChanged(Func<IReadOnlyCollection<PersonStateTicket>, Task> changed);
    
    IDisposable OnSurveyStateChanged(Func<long, bool, Task> changed);

    IDisposable OnSurveyFinished(Func<Task> changed);
}