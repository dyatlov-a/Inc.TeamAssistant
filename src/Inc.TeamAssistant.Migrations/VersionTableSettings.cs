using FluentMigrator.Runner.VersionTableInfo;

namespace Inc.TeamAssistant.Migrations;

internal sealed class VersionTableSettings : IVersionTableMetaData
{
    public bool OwnsSchema => true;
    public string SchemaName => "versioning";
    public string TableName => "migrations";
    public string ColumnName => "version";
    public string UniqueIndexName => "migrations__uidx__version";
    public string DescriptionColumnName => "description";
    public string AppliedOnColumnName => "applied";
    public bool CreateWithPrimaryKey => false;
}
