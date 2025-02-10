using System.Text;
using FluentValidation;

namespace Inc.TeamAssistant.Connector.Application.Executors;

internal static class ValidationMessageBuilder
{
    public static string Build(ValidationException validationException)
    {
        ArgumentNullException.ThrowIfNull(validationException);

        if (validationException.Errors.Any())
            return validationException.Errors.Aggregate(
                new StringBuilder(),
                (sb, e) =>
                {
                    sb.AppendLine(e.ErrorMessage);
                    return sb;
                },
                sb => sb.ToString());
        
        return validationException.Message;
    }
}