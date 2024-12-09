using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_12_09_0)]
public sealed class ChangeReviewer  : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            UPDATE review.task_for_reviews AS t
            SET strategy = 3
            WHERE t.has_concrete_reviewer
            """,
            "Set strategy by has_concrete_reviewer");
        
        Delete
            .Column("has_concrete_reviewer")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }

    public override void Down()
    {
        Create
            .Column("has_concrete_reviewer")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
        
        Execute.Sql(
            """
            UPDATE review.task_for_reviews AS t
            SET has_concrete_reviewer = t.strategy = 3;
            """,
            "Set has_concrete_reviewer by strategy");
    }
}