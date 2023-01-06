using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Validators;

internal sealed class ModeratorValidator : AbstractValidator<IWithModerator>
{
    public ModeratorValidator()
    {
        RuleFor(e => e.ModeratorName).NotEmpty();
    }
}