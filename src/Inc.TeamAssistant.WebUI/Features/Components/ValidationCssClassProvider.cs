using Microsoft.AspNetCore.Components.Forms;

namespace Inc.TeamAssistant.WebUI.Features.Components;

internal sealed class ValidationCssClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        ArgumentNullException.ThrowIfNull(editContext);

        const string isValidCssClass = "is-valid";
        const string isInvalidCssClass = "is-invalid";
        
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (editContext.IsModified(fieldIdentifier))
            return isValid ? isValidCssClass : isInvalidCssClass;
        
        return isValid ? string.Empty : isInvalidCssClass;
    }
}