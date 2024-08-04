using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_08_04_0)]
public sealed class AddDraftTaskForReview : Migration
{
    public override void Up()
    {
        Create
            .Table("draft_task_for_reviews")
            .InSchema("review")

            .WithColumn("id")
            .AsGuid()
            .NotNullable()
            .PrimaryKey("draft_task_for_reviews__pk__id")
            
            .WithColumn("team_id")
            .AsGuid()
            .NotNullable()
            
            .WithColumn("owner_id")
            .AsInt64()
            .NotNullable()
            
            .WithColumn("strategy")
            .AsInt32()
            .NotNullable()
            
            .WithColumn("chat_id")
            .AsInt64()
            .NotNullable()
            
            .WithColumn("message_id")
            .AsInt32()
            .NotNullable()
            
            .WithColumn("description")
            .AsString(2000)
            .NotNullable()
            
            .WithColumn("target_person_id")
            .AsInt64()
            .Nullable()
            
            .WithColumn("preview_message_id")
            .AsInt32()
            .Nullable()
            
            .WithColumn("created")
            .AsDateTimeOffset()
            .NotNullable();
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_commands(id, value, help_message_id, scopes)
            VALUES
                ('80421e0b-20f5-4bd4-9a78-29bedce291f7', '/move_to_review', null, '[]'::jsonb),
                ('fc2567b7-e79e-4b70-a543-fb77349bfe54', '/remove_draft', null, '[]'::jsonb),
                ('eccea629-8f66-425b-a28d-ffad1a361441', '/edit_draft', null, '[]'::jsonb)
            """,
            "Create preview commands");
        
        Execute.Sql(
            """
            INSERT INTO connector.command_packs(feature_id, bot_command_id)
            VALUES
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '80421e0b-20f5-4bd4-9a78-29bedce291f7'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'fc2567b7-e79e-4b70-a543-fb77349bfe54'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'eccea629-8f66-425b-a28d-ffad1a361441')
            """,
            "Add preview commands to packs");
    }

    public override void Down()
    {
        Execute.Sql(
            """
            DELETE FROM connector.bot_commands
            WHERE id IN (
                '80421e0b-20f5-4bd4-9a78-29bedce291f7',
                'fc2567b7-e79e-4b70-a543-fb77349bfe54',
                'eccea629-8f66-425b-a28d-ffad1a361441');
            """,
            "Delete preview commands");
        
        Delete
            .Table("draft_task_for_reviews")
            .InSchema("review");
    }
}