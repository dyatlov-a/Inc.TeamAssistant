using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_04_14_0)]
public sealed class ChangeTeams : Migration
{
    public override void Up()
    {
        Execute.Sql(
            "UPDATE connector.teams SET name = replace(name, ' ', '_');",
            "Remove spaces from team names");
    }

    public override void Down()
    {
    }
}