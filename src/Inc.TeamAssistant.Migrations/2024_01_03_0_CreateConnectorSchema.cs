using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_01_03_0)]
public sealed class CreateConnectorSchema : Migration
{
    public override void Up()
    {
        Create
            .Schema("connector");

        Execute.Sql(
            "grant usage on schema connector to team_assistant__api;",
            "add permissions on usage connector schema to team_assistant__api user");

        Execute.Sql(
            "alter default privileges in schema connector grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in connector for team_assistant__api user");
    }

    public override void Down()
    {
        Delete
            .Schema("connector");
    }
}