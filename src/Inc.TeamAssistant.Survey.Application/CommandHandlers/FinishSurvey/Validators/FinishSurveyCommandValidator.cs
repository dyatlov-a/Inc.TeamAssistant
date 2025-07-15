using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Rooms;
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
        
        RuleFor(c => c.SurveyId)
            .NotEmpty();
        
        RuleFor(c => c.RoomId)
            .NotEmpty();
        
        RuleFor(e => e)
            .MustAsync(HasRights)
            .WithMessage("You do not have rights to change this action item.");
    }
    
    private async Task<bool> HasRights(FinishSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var properties = await _propertiesProvider.Get(command.RoomId, token);

        return currentPerson.Id == properties.FacilitatorId;
    }
}