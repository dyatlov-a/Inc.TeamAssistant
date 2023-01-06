using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Validators;

internal sealed class ReVoteEstimateCommandValidator : AbstractValidator<ReVoteEstimateCommand>
{
    public ReVoteEstimateCommandValidator(IValidator<IWithModerator> moderatorValidator)
    {
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));

        RuleFor(e => e).SetValidator(moderatorValidator);
    }
}