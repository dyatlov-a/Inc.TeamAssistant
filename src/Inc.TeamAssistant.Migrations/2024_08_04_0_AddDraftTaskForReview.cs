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
    }

    public override void Down()
    {
        Delete
            .Table("draft_task_for_reviews")
            .InSchema("review");
    }
}