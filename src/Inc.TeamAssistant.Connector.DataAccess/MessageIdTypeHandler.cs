using System.Data;
using Dapper;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class MessageIdTypeHandler : SqlMapper.TypeHandler<MessageId>
{
    public override void SetValue(IDbDataParameter parameter, MessageId? messageId)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        parameter.DbType = DbType.String;
        parameter.Value = messageId?.Value;
    }

    public override MessageId Parse(object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new((string)value);
    }
}