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
            "grant usage on schema random_coffee to appraiser__api;",
            "add permissions on usage random_coffee schema to appraiser__api user");

        Execute.Sql(
            "alter default privileges in schema random_coffee grant select, update, insert, delete on tables to appraiser__api;",
            "add select, update, insert privileges to all tables in random_coffee for appraiser__api user");
    }

    public override void Down()
    {
        Delete
            .Schema("random_coffee");
    }
}