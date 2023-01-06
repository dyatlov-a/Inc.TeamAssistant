using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AllowUseName.Validators;

internal sealed class AllowUseNameCommandValidator : AbstractValidator<AllowUseNameCommand>
{
    public AllowUseNameCommandValidator(IValidator<IWithAppraiser> appraiserValidator)
    {
        if (appraiserValidator is null)
            throw new ArgumentNullException(nameof(appraiserValidator));

        RuleFor(e => e).SetValidator(appraiserValidator);
    }
}