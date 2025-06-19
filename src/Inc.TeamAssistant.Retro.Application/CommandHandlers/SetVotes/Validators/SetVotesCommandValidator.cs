using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetVotes.Validators;

internal sealed class SetVotesCommandValidator : AbstractValidator<SetVotesCommand>
{
    public SetVotesCommandValidator(IValidator<PersonVoteByItemDto> voteValidator)
    {
        ArgumentNullException.ThrowIfNull(voteValidator);

        RuleFor(c => c.RoomId)
            .NotEmpty();
        
        RuleFor(c => c.Votes)
            .ForEach(c => c.SetValidator(voteValidator));
    }
}