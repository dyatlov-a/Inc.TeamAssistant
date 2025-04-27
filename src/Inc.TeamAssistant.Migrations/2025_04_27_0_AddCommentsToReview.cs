using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_04_27_0)]
public sealed class AddCommentsToReview : Migration
{
    public override void Up()
    {
        Create
            .Column("comments")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsCustom("jsonb").NotNullable().SetExistingRowsTo("[]");
    }

    public override void Down()
    {
        Delete
            .Column("comments")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}