using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_01_10_0)]
public sealed class CreateAppraiserScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("appraiser");

        Execute.Sql(
            "grant usage on schema appraiser to appraiser__api;",
            "add permissions on usage appraiser schema to appraiser__api user");

        Execute.Sql(
            "alter default privileges in schema appraiser grant select, update, insert, delete on tables to appraiser__api;",
            "add select, update, insert privileges to all tables in appraiser for appraiser__api user");
    }

    public override void Down()
    {
        Delete
            .Schema("appraiser");
    }
}