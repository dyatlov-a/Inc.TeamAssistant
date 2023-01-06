using System.Data;
using Dapper;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed class LanguageIdTypeHandler : SqlMapper.TypeHandler<LanguageId>
{
    public override void SetValue(IDbDataParameter parameter, LanguageId languageId)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));

        parameter.DbType = DbType.String;
        parameter.Value = languageId.Value;
    }

    public override LanguageId Parse(object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new((string)value);
    }
}