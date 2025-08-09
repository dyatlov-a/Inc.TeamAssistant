namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyEventSender
{
    Task SurveyStarted(Guid roomId);
    
    Task SurveyStateChanged(Guid roomId, long personId, bool finished);

    Task SurveyFinished(Guid roomId);
}