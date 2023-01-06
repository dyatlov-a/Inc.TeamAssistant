namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IAssessmentSessionMetrics
{
    void Created();

    void Started();

    void Ended();
}