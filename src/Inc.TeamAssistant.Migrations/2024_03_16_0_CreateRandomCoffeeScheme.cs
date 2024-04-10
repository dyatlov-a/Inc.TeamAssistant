using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_03_16_0)]
public sealed class CreateRandomCoffeeScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("random_coffee");

        Execute.Sql(
            "grant usage on schema random_coffee to team_assistant__api;",
            "add permissions on usage random_coffee schema to team_assistant__api user");

        Execute.Sql(
            "alter default privileges in schema random_coffee grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in random_coffee for team_assistant__api user");
    }

    public override void Down()
    {
        Delete
            .Schema("random_coffee");
    }
}