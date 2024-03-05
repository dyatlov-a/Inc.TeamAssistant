using System.Text;
using FluentValidation;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class ValidationExceptionExtensions
{
    public static string ToMessage(this ValidationException validationException)
    {
        if (validationException is null)
            throw new ArgumentNullException(nameof(validationException));

        return validationException.Errors.Any()
            ? validationException.Errors.Aggregate(
                new StringBuilder(),
                (sb, e) =>
                {
                    sb.AppendLine(e.ErrorMessage);
                    return sb;
                },
                sb => sb.ToString())
            : validationException.Message;
    }
}