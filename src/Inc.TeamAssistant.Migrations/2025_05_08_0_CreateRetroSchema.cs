using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_05_08_0)]
public sealed class CreateRetroSchema : Migration
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
            .Table("retro_sessions")
            .InSchema("retro")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("retro_sessions__pk__id")

            .WithColumn("team_id")
            .AsGuid().NotNullable()

            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()

            .WithColumn("state")
            .AsInt32().NotNullable()

            .WithColumn("facilitator_id")
            .AsInt64().NotNullable();
        
        Create
            .Table("retro_items")
            .InSchema("retro")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("retro_items__pk__id")
            
            .WithColumn("team_id")
            .AsGuid().NotNullable()
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("column_id")
            .AsGuid().NotNullable()
            
            .WithColumn("position")
            .AsInt32().NotNullable()

            .WithColumn("text")
            .AsString().Nullable()

            .WithColumn("owner_id")
            .AsInt64().NotNullable()
            
            .WithColumn("retro_session_id")
            .AsGuid().Nullable()
            .ForeignKey("retro_items__fk__rs_id", "retro", "retro_sessions", "id")
            
            .WithColumn("parent_id")
            .AsGuid().Nullable()
            
            .WithColumn("votes")
            .AsInt32().Nullable();

        Execute.Sql(
            """
            CREATE UNIQUE INDEX retro_sessions__uidx__t_id__active
            ON retro.retro_sessions (team_id)
            WHERE state != 4;
            """,
            "Create unique index on retro_sessions for active sessions");
    }

    public override void Down()
    {
        Delete
            .Table("retro_items")
            .InSchema("retro");
        
        Delete
            .Table("retro_sessions")
            .InSchema("retro");
        
        Delete
            .Schema("retro");
    }
}