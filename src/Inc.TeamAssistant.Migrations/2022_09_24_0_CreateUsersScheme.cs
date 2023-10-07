using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2022_09_24_0)]
public sealed class CreateUsersScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("users");

        Execute.Sql(
            "grant usage on schema users to appraiser__api;",
            "add permissions on usage users schema to appraiser__api user"
        );

        Execute.Sql(
            "alter default privileges in schema users grant select, update, insert, delete on tables to appraiser__api;",
            "add select, update, insert privileges to all tables in users for appraiser__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("users");
    }
}