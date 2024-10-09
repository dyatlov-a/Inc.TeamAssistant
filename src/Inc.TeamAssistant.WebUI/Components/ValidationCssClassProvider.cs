using Microsoft.AspNetCore.Components.Forms;

namespace Inc.TeamAssistant.WebUI.Components;

internal sealed class ValidationCssClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        ArgumentNullException.ThrowIfNull(editContext);

        const string isValidCssClass = "input_valid";
        const string isInvalidCssClass = "input_invalid";
        
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (editContext.IsModified(fieldIdentifier))
            return isValid ? isValidCssClass : isInvalidCssClass;
        
        return isValid ? string.Empty : isInvalidCssClass;
    }
}