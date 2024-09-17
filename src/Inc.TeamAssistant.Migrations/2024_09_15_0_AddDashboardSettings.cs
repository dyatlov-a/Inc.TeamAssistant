using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_09_15_0)]
public sealed class AddDashboardSettings : Migration
{
    public override void Up()
    {
        Create
            .Table("dashboard_settings")
            .InSchema("connector")

            .WithColumn("person_id")
            .AsInt64().NotNullable()
            .PrimaryKey("dashboard_settings__pk__person_id__bot_id")

            .WithColumn("bot_id")
            .AsGuid().NotNullable()
            .PrimaryKey("dashboard_settings__pk__person_id__bot_id")

            .WithColumn("widgets")
            .AsCustom("jsonb").NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("dashboard_settings")
            .InSchema("connector");
    }
}