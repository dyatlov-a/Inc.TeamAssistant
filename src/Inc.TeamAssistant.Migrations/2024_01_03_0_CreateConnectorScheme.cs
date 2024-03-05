using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_01_03_0)]
public sealed class CreateConnectorScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("connector");

        Execute.Sql(
            "grant usage on schema connector to appraiser__api;",
            "add permissions on usage connector schema to appraiser__api user");

        Execute.Sql(
            "alter default privileges in schema connector grant select, update, insert, delete on tables to appraiser__api;",
            "add select, update, insert privileges to all tables in connector for appraiser__api user");
    }

    public override void Down()
    {
        Delete
            .Schema("connector");
    }
}