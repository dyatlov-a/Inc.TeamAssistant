using FluentValidation;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Validators;

internal sealed class AddLocationToMapCommandValidator : AbstractValidator<AddLocationToMapCommand>
{
    public AddLocationToMapCommandValidator()
    {
        RuleFor(c => c.MessageContext.Location)
            .NotEmpty();
    }
}