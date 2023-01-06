using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Validators;

internal sealed class AcceptEstimateCommandValidator : AbstractValidator<AcceptEstimateCommand>
{
    public AcceptEstimateCommandValidator(IValidator<IWithModerator> moderatorValidator)
    {
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));

        RuleFor(e => e).SetValidator(moderatorValidator);
    }
}