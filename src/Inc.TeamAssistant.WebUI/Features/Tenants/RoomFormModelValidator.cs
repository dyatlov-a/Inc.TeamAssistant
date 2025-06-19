using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Tenants;

public sealed class RoomFormModelValidator : AbstractValidator<RoomFormModel>
{
    public RoomFormModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}