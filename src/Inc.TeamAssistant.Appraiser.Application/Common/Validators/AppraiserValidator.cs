using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Validators;

internal sealed class AppraiserValidator : AbstractValidator<IWithAppraiser>
{
    public AppraiserValidator()
    {
        RuleFor(e => e.AppraiserName).NotEmpty();
    }
}