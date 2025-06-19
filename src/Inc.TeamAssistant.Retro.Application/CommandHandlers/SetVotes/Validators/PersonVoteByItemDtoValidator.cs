using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetVotes.Validators;

internal sealed class PersonVoteByItemDtoValidator : AbstractValidator<PersonVoteByItemDto>
{
    public PersonVoteByItemDtoValidator()
    {
        RuleFor(c => c.ItemId)
            .NotEmpty();

        RuleFor(c => c.Vote)
            .NotEmpty();
    }
}