using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishEstimate;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishEstimate.Validators;

internal sealed class FinishEstimateCommandValidator : AbstractValidator<FinishEstimateCommand>
{
    public FinishEstimateCommandValidator()
    {
        RuleFor(e => e.StoryId)
            .NotEmpty();
    }
}