using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_05_08_0)]
public sealed class CreateRetroScheme : Migration
{
    public override void Up()
    {
        Create
            .Schema("retro");

        Execute.Sql(
            "grant usage on schema retro to team_assistant__api;",
            "add permissions on usage retro schema to team_assistant__api user");

        Execute.Sql(
            "alter default privileges in schema retro grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in retro for team_assistant__api user");

        Create
            .Table("retro_items")
            .InSchema("retro")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("items__pk__id")
            
            .WithColumn("team_id")
            .AsGuid().NotNullable()
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("type")
            .AsInt32().NotNullable()

            .WithColumn("text")
            .AsString(1000).Nullable()

            .WithColumn("owner_id")
            .AsInt64().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("retro_items")
            .InSchema("retro");
        
        Delete
            .Schema("retro");
    }
}