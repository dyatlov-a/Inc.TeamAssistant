using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2022_12_21_0)]
public sealed class CreateReviewScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("review");

        Execute.Sql(
            "grant usage on schema review to appraiser__api;",
            "add permissions on usage review schema to appraiser__api user"
        );

        Execute.Sql(
            "alter default privileges in schema review grant select, update, insert, delete on tables to appraiser__api;",
            "add select, update, insert privileges to all tables in review for appraiser__api user"
        );
    }

    public override void Down()
    {
        Delete
            .Schema("review");
    }
}