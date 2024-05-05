using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBotUserName.Validators;

internal sealed class GetBotUserNameQueryValidator : AbstractValidator<GetBotUserNameQuery>
{
    public GetBotUserNameQueryValidator()
    {
        RuleFor(e => e.Token).NotEmpty();
    }
}