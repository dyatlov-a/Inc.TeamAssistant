using System.Data;
using Dapper;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives.DataAccess.Internal;

internal sealed class MessageIdTypeHandler : SqlMapper.TypeHandler<MessageId>
{
    public override void SetValue(IDbDataParameter parameter, MessageId? messageId)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        parameter.DbType = DbType.String;
        parameter.Value = messageId?.Value;
    }

    public override MessageId Parse(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new((string)value);
    }
}