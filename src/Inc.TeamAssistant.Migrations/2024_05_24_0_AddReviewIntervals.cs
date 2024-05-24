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
    }

    public override void Down()
    {
        Delete
            .Column("review_intervals")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}