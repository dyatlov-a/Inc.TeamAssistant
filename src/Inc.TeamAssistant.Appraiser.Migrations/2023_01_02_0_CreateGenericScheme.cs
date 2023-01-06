using FluentMigrator;

namespace Inc.TeamAssistant.Appraiser.Migrations;

[Migration(2023_01_02_0)]
public sealed class CreateGenericScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("generic");

        Execute.Sql(
            "grant usage on schema generic to appraiser__api;",
            "add permissions on usage generic schema to appraiser__api user"
        );

        Execute.Sql(
            "alter default privileges in schema generic grant select, update, insert, delete on tables to appraiser__api;",
            "add select, update, insert privileges to all tables in generic for appraiser__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("generic");
    }
}