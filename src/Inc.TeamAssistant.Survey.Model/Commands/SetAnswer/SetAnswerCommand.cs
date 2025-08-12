using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;

public sealed record SetAnswerCommand(
    Guid SurveyId,
    Guid QuestionId,
    int? Value,
    string? Comment,
    bool IsFinal)
    : IRequest;