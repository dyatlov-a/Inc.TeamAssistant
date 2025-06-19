using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2023_01_02_0)]
public sealed class CreateGenericSchema : Migration
{
    public override void Up()
    {
        Create
            .Schema("generic");

        Execute.Sql(
            "grant usage on schema generic to team_assistant__api;",
            "add permissions on usage generic schema to team_assistant__api user"
        );

        Execute.Sql(
            "alter default privileges in schema generic grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in generic for team_assistant__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("generic");
    }
}