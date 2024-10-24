using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.Extensions.Localization;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;

internal sealed class CheckBotFormModelValidator : AbstractValidator<CheckBotFormModel>
{
    private readonly IBotService _botService;
    private readonly IStringLocalizer<ConstructorResources> _localizer;
    
    public CheckBotFormModelValidator(IBotService botService, IStringLocalizer<ConstructorResources> localizer)
    {
        _botService = botService ?? throw new ArgumentNullException(nameof(botService));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        RuleFor(e => e.Token)
            .NotEmpty();
            
        RuleFor(e => e.Token)
            .CustomAsync(CheckToken);

        RuleFor(e => e.UserName)
            .NotEmpty();
    }
    
    private async Task CheckToken(
        string botToken,
        ValidationContext<CheckBotFormModel> context,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(botToken))
            return;
        
        var result = await _botService.Check(new GetBotUserNameQuery(botToken), token);

        if (!result.HasAccess)
            context.AddFailure(nameof(CheckBotFormModel.Token), _localizer["TokenInvalid"]);
    }
}