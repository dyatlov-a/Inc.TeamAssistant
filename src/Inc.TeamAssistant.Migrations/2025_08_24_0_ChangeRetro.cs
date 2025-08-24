using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_08_24_0)]
public sealed class ChangeRetro : Migration
{
    public override void Up()
    {
        Create
            .Column("template_id")
            .OnTable("retro_sessions")
            .InSchema("retro")

            .AsGuid()
            .Nullable();
        
        Execute.Sql(
            """
            UPDATE retro.retro_sessions AS t
            SET template_id = (r.properties ->> 'RetroTemplateId')::uuid
            FROM retro.retro_sessions AS s
            JOIN tenants.rooms AS r ON s.room_id = r.id
            WHERE t.id = s.id;
            """,
            "Set template_id for retro.retro_sessions");

        Alter
            .Column("template_id")
            .OnTable("retro_sessions")
            .InSchema("retro")
            .AsGuid()
            .NotNullable();
    }

    public override void Down()
    {
        Delete
            .Column("template_id")
            .FromTable("retro_sessions")
            .InSchema("retro");
    }
}