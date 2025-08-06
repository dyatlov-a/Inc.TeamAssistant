using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Features.Survey;

public interface ISurveyEventProvider
{
    IDisposable OnFacilitatorChanged(Func<long, Task> changed);

    IDisposable OnPersonsChanged(Func<IReadOnlyCollection<Person>, Task> changed);
}