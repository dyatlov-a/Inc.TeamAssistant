using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SettingsFormModelTracker : ComponentBase, IDisposable
{
    [Parameter, EditorRequired]
    public StagesState StagesState { get; set; } = default!;
    
    [Parameter, EditorRequired]
    public Action OnChange { get; set; } = default!;
    
    [CascadingParameter]
    private EditContext CascadedEditContext { get; set; } = default!;
    
    protected override void OnInitialized()
    {
        if (CascadedEditContext is null)
            throw new InvalidOperationException($"{nameof(SettingsFormModelTracker)} requires a cascading parameter of type {nameof(EditContext)}");

        CascadedEditContext.OnFieldChanged += OnFieldChanged;
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        var model = (SettingsFormModel)e.FieldIdentifier.Model;

        StagesState.Apply(model);
        
        OnChange();
    }

    public void Dispose() => CascadedEditContext.OnFieldChanged -= OnFieldChanged;
}