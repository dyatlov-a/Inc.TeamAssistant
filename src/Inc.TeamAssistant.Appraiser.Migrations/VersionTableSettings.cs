using FluentMigrator.Runner.VersionTableInfo;

namespace Inc.TeamAssistant.Appraiser.Migrations;

internal sealed class VersionTableSettings : IVersionTableMetaData
{
    public object ApplicationContext
    {
        get => null!;
        set { } // do nothing
    }

    public bool OwnsSchema => true;

    public string SchemaName
    {
        get => "versioning";
        set { } // deny this
    }

    public string TableName => "migrations";
    public string ColumnName => "version";
    public string UniqueIndexName => "migrations__uidx__version";
    public string DescriptionColumnName => "description";
    public string AppliedOnColumnName => "applied";
}
