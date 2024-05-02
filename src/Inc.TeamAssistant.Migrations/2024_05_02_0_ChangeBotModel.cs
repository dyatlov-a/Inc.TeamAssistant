using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_05_02_0)]
public sealed class ChangeBotModel : Migration
{
    public override void Up()
    {
        Delete
            .Column("name")
            .FromTable("bots")
            .InSchema("connector");
    }

    public override void Down()
    {
    }
}