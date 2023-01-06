using FluentMigrator;

namespace Inc.TeamAssistant.Appraiser.Migrations;

[Migration(2022_12_12_0)]
public sealed class CreateMapsScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("maps");

        Execute.Sql(
            "grant usage on schema maps to appraiser__api;",
            "add permissions on usage maps schema to appraiser__api user"
        );

        Execute.Sql(
            "alter default privileges in schema maps grant select, update, insert, delete on tables to appraiser__api;",
            "add select, update, insert privileges to all tables in maps for appraiser__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("maps");
    }
}