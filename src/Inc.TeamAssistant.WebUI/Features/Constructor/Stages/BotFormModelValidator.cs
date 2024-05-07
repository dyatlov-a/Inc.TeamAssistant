using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

internal sealed class BotFormModelValidator : AbstractValidator<BotFormModel>
{
    private readonly IBotService _botService;
    
    public BotFormModelValidator(IBotService botService)
    {
        _botService = botService ?? throw new ArgumentNullException(nameof(botService));
        
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Please enter a token.")
            .MustAsync(CheckAccessToBot)
            .WithMessage("Token was not valid. Please check it.");
    }
    
    private async Task<bool> CheckAccessToBot(string botToken, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(botToken))
            return false;
        
        var getBotUserNameResult = await _botService.Check(new GetBotUserNameQuery(botToken), token);

        return getBotUserNameResult.Result.HasAccess;
    }
}