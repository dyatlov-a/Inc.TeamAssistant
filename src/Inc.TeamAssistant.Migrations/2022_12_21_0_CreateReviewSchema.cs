using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2022_12_21_0)]
public sealed class CreateReviewSchema : Migration
{
    public override void Up()
    {
        Create
            .Schema("review");

        Execute.Sql(
            "grant usage on schema review to team_assistant__api;",
            "add permissions on usage review schema to team_assistant__api user"
        );

        Execute.Sql(
            "alter default privileges in schema review grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in review for team_assistant__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("review");
    }
}