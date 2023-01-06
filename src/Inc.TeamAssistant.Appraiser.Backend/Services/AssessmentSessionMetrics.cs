using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Prometheus;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

internal sealed class AssessmentSessionMetrics : IAssessmentSessionMetrics
{
    private static readonly Counter AssessmentSessionState = Metrics.CreateCounter(
        "assessment_session",
        "Assessment session state",
        new CounterConfiguration
        {
            LabelNames = new[] { "state" }
        });

    public void Created() => AssessmentSessionState.WithLabels("created").Inc();

    public void Started() => AssessmentSessionState.WithLabels("started").Inc();

    public void Ended() => AssessmentSessionState.WithLabels("ended").Inc();
}