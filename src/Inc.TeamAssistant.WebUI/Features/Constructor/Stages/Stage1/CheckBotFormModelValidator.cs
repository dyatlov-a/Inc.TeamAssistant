using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;

public sealed class CheckBotFormModelValidator : AbstractValidator<CheckBotFormModel>
{
    private readonly IBotService _botService;
    private readonly ResourcesManager _resourcesManager;
    
    public CheckBotFormModelValidator(IBotService botService, ResourcesManager resourcesManager)
    {
        _botService = botService ?? throw new ArgumentNullException(nameof(botService));
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));

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
            context.AddFailure(nameof(CheckBotFormModel.Token), _resourcesManager[Messages.Validation_TokenInvalid]);
    }
}