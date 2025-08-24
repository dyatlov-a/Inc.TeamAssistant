using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
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
            .MustAsync(HasFacilitationRights)
            .WithMessage(c => $"You do not have facilitation rights for room {c.RoomId}.")
            .MustAsync(NotHaveActiveSurvey)
            .WithMessage(c => $"There is already an active survey for this room {c.RoomId}.");
    }

    private async Task<bool> NotHaveActiveSurvey(Guid roomId, CancellationToken token)
    {
        var survey = await _reader.ReadLastSurvey(roomId, SurveyStateRules.Active, token);
        
        return survey is null;
    }
    
    private async Task<bool> HasFacilitationRights(Guid roomId, CancellationToken token)
    {
        var properties = await _propertiesProvider.Get(roomId, token);
        var currentPerson = _personResolver.GetCurrentPerson();

        return properties.FacilitatorId == currentPerson.Id;
    }
}