using FluentValidation;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.StartSurvey.Validators;

internal sealed class StartSurveyCommandValidator : AbstractValidator<StartSurveyCommand>
{
    private readonly ISurveyReader _reader;
    
    public StartSurveyCommandValidator(ISurveyReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        
        RuleFor(c => c.RoomId)
            .NotEmpty()
            .MustAsync(NotHaveActiveSurvey)
            .WithMessage(c => $"There is already an active survey for this room {c.RoomId}.");
        
        RuleFor(c => c.TemplateId)
            .NotEmpty();
    }

    private async Task<bool> NotHaveActiveSurvey(Guid roomId, CancellationToken token)
    {
        var survey = await _reader.Find(roomId, SurveyStateRules.Active, token);
        
        return survey is null;
    }
}