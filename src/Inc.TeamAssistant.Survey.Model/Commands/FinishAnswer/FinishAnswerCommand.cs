using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Commands.FinishAnswer;

public sealed record FinishAnswerCommand(Guid SurveyId)
    : IRequest;