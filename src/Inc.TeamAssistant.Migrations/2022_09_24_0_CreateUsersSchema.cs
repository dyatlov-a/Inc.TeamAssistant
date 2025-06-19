using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2022_09_24_0)]
public sealed class CreateUsersSchema : Migration
{
    public override void Up()
    {
        Create
            .Schema("users");

        Execute.Sql(
            "grant usage on schema users to team_assistant__api;",
            "add permissions on usage users schema to team_assistant__api user"
        );

        Execute.Sql(
            "alter default privileges in schema users grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in users for team_assistant__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("users");
    }
}