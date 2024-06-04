using Microsoft.AspNetCore.Components.Forms;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

internal static class EditContextFactory
{
    public static EditContext Create(object model)
    {
        ArgumentNullException.ThrowIfNull(model);
        
        var editContext = new EditContext(model);
        editContext.SetFieldCssClassProvider(new ValidationCssClassProvider());
        
        return editContext;
    }
}