using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_06_20_0)]
public sealed class AddNameToMap : Migration
{
    public override void Up()
    {
        Create
            .Column("name")
            .OnTable("maps")
            .InSchema("maps")
            .AsString(128).Nullable();
    }

    public override void Down()
    {
        Delete
            .Column("name")
            .FromTable("maps")
            .InSchema("maps");
    }
}