using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_05_24_0)]
public sealed class AddReviewIntervals : Migration
{
    public override void Up()
    {
        Create
            .Column("review_intervals")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsCustom("jsonb").NotNullable().SetExistingRowsTo("[]");
        
        Create
            .Column("owner_message_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt32().Nullable();

        Create
            .Column("reviewer_message_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt32().Nullable();
    }

    public override void Down()
    {
        Delete
            .Column("reviewer_message_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("owner_message_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("review_intervals")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}