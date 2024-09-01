using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;

public sealed class CheckBotFormTracker : ComponentBase, IDisposable
{
    [Inject]
    public IBotService BotService { get; set; } = default!;
    
    [Parameter, EditorRequired]
    public Action OnChange { get; set; } = default!;
    
    [CascadingParameter]
    private EditContext CascadedEditContext { get; set; } = default!;
    private CheckBotFormModel? _oldValues;

    protected override void OnInitialized()
    {
        if (CascadedEditContext is null)
            throw new InvalidOperationException($"{nameof(CheckBotFormTracker)} requires a cascading parameter of type {nameof(EditContext)}");

        CascadedEditContext.OnFieldChanged += OnFieldChanged;
    }

    private async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        var model = (CheckBotFormModel)e.FieldIdentifier.Model;

        if (_oldValues is not null &&
            _oldValues.HasAccess == model.HasAccess &&
            _oldValues.Token == model.Token &&
            _oldValues.UserName == model.UserName)
            return;
        
        if (string.IsNullOrWhiteSpace(model.Token))
        {
            model.UserName = string.Empty;
            model.HasAccess = false;
        }
        else
        {
            var getBotUserNameResult = await BotService.Check(new GetBotUserNameQuery(model.Token));
            model.UserName = getBotUserNameResult.UserName;
            model.HasAccess = getBotUserNameResult.HasAccess;
        }

        _oldValues = new CheckBotFormModel
        {
            HasAccess = model.HasAccess,
            Token = model.Token,
            UserName = model.UserName
        };
        
        CascadedEditContext.NotifyFieldChanged(e.FieldIdentifier);
        CascadedEditContext.NotifyFieldChanged(new FieldIdentifier(model, nameof(model.UserName)));
        OnChange();
    }

    public void Dispose() => CascadedEditContext.OnFieldChanged -= OnFieldChanged;
}