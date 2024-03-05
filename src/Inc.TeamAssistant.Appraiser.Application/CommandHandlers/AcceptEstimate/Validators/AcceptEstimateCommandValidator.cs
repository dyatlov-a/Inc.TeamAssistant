using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Validators;

internal sealed class AcceptEstimateCommandValidator : AbstractValidator<AcceptEstimateCommand>
{
    public AcceptEstimateCommandValidator()
    {
        RuleFor(e => e.StoryId)
            .NotEmpty();
    }
}