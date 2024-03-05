using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Validators;

internal sealed class ReVoteEstimateCommandValidator : AbstractValidator<ReVoteEstimateCommand>
{
    public ReVoteEstimateCommandValidator()
    {
        RuleFor(e => e.StoryId)
            .NotEmpty();
    }
}