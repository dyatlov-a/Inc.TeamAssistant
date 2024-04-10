using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2022_12_12_0)]
public sealed class CreateMapsScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("maps");

        Execute.Sql(
            "grant usage on schema maps to team_assistant__api;",
            "add permissions on usage maps schema to team_assistant__api user"
        );

        Execute.Sql(
            "alter default privileges in schema maps grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in maps for team_assistant__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("maps");
    }
}