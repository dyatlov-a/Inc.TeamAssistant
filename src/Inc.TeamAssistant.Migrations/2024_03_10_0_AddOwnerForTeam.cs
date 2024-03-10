using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_03_10_0)]
public sealed class AddOwnerForTeam : Migration
{
    public override void Up()
    {
        Create
            .Column("owner_id")
            .OnTable("teams")
            .InSchema("connector")
            .AsInt64().NotNullable().SetExistingRowsTo(272062137);
    }

    public override void Down()
    {
        Delete
            .Column("owner_id")
            .FromTable("teams")
            .InSchema("connector");
    }
}