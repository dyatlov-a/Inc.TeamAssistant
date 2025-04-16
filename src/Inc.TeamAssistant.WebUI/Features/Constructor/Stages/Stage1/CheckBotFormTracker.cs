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

    protected override void OnInitialized()
    {
        if (CascadedEditContext is null)
            throw new InvalidOperationException($"{nameof(CheckBotFormTracker)} requires a cascading parameter of type {nameof(EditContext)}");

        CascadedEditContext.OnFieldChanged += OnFieldChanged;
    }
    
    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (e.FieldIdentifier.FieldName != nameof(CheckBotFormModel.Token))
            return;
        
        var model = (CheckBotFormModel)e.FieldIdentifier.Model;
        
        Task.Run(() => CheckToken(model));
    }

    private async Task CheckToken(CheckBotFormModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (string.IsNullOrWhiteSpace(model.Token))
            model.Clear();
        else
        {
            var getBotUserNameResult = await BotService.Check(new GetBotUserNameQuery(model.Token));
            model.Apply(getBotUserNameResult);
        }
        
        OnChange();
    }

    public void Dispose() => CascadedEditContext.OnFieldChanged -= OnFieldChanged;
}