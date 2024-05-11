using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;

public sealed class CheckBotFormModelValidator : AbstractValidator<CheckBotFormModel>
{
    private readonly IBotService _botService;
    
    public CheckBotFormModelValidator(IBotService botService)
    {
        _botService = botService ?? throw new ArgumentNullException(nameof(botService));
        
        RuleFor(e => e.Token)
            .NotEmpty()
            .MustAsync(CheckToken)
            .WithMessage("The token is invalid.");

        RuleFor(e => e.UserName)
            .NotEmpty();
    }
    
    private async Task<bool> CheckToken(string botToken, CancellationToken token)
    {
        var result = await _botService.Check(new GetBotUserNameQuery(botToken), token);

        return result.Result.HasAccess;
    }
}