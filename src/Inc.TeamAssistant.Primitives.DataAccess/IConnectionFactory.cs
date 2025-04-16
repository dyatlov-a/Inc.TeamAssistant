using System.Data.Common;

namespace Inc.TeamAssistant.Primitives.DataAccess;

public interface IConnectionFactory
{
    DbConnection Create();
}