using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_01_11_1)]
public sealed class ChangeCheckInTables : Migration
{
    public override void Up()
    {
        Delete
            .Column("data")
            .FromTable("locations")
            .InSchema("maps");
    }

    public override void Down()
    {
    }
}