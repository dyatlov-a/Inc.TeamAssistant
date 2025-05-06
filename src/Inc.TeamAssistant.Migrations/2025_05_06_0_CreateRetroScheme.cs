using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_05_06_0)]
public sealed class CreateRetroScheme : Migration
{
    public override void Up()
    {
        Create.Schema("retro");

        Execute.Sql(
            "grant usage on schema users to team_assistant__api;",
            "add permissions on usage users schema to team_assistant__api user");

        Execute.Sql(
            "alter default privileges in schema users grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in users for team_assistant__api user");

        Create
            .Table("retro_card_pools")
            .InSchema("retro")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("retro_card_pools__pk__id")

            .WithColumn("name")
            .AsString(50).NotNullable()

            .WithColumn("owner_id")
            .AsInt64().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("retro_card_pools")
            .InSchema("retro");
        
        Delete.Schema("retro");
    }
}