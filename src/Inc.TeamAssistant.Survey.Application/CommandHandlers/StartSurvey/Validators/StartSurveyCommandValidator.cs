using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.StartSurvey.Validators;

internal sealed class StartSurveyCommandValidator : AbstractValidator<StartSurveyCommand>
{
    private readonly ISurveyReader _reader;
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    
    public StartSurveyCommandValidator(
        ISurveyReader reader,
        IRoomPropertiesProvider propertiesProvider,
        IPersonResolver personResolver)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(c => c.RoomId)
            .NotEmpty()
            .MustAsync(NotHaveActiveSurvey)
            .WithMessage(c => $"There is already an active survey for this room {c.RoomId}.");
        
        RuleFor(c => c.TemplateId)
            .NotEmpty();
        
        RuleFor(e => e)
            .MustAsync(HasRights)
            .WithMessage("You do not have rights to change this action item.");
    }

    private async Task<bool> NotHaveActiveSurvey(Guid roomId, CancellationToken token)
    {
        var survey = await _reader.Find(roomId, SurveyStateRules.Active, token);
        
        return survey is null;
    }
    
    private async Task<bool> HasRights(StartSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var properties = await _propertiesProvider.Get(command.RoomId, token);

        return currentPerson.Id == properties.FacilitatorId;
    }
}