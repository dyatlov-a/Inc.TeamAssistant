using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.FinishSurvey.Validators;

internal sealed class FinishSurveyCommandValidator : AbstractValidator<FinishSurveyCommand>
{
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    
    public FinishSurveyCommandValidator(IRoomPropertiesProvider propertiesProvider, IPersonResolver personResolver)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(c => c.RoomId)
            .NotEmpty()
            .MustAsync(HasFacilitationRights)
            .WithMessage(c => $"You do not have facilitation rights for room {c.RoomId}.");
    }
    
    private async Task<bool> HasFacilitationRights(Guid roomId, CancellationToken token)
    {
        var properties = await _propertiesProvider.Get(roomId, token);
        var currentPerson = _personResolver.GetCurrentPerson();

        return properties.FacilitatorId == currentPerson.Id;
    }
}