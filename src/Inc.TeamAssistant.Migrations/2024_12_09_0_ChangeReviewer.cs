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
        
        Execute.Sql(
            """
            UPDATE review.task_for_reviews AS t
            SET state = 5
            WHERE t.accepted_with_comments
            """,
            "Set state by accepted_with_comments");
        
        Delete
            .Column("accepted_with_comments")
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
        
        Execute.Sql(
            """
            UPDATE review.task_for_reviews
            SET strategy = 2
            WHERE strategy = 3;
            """,
            "Change strategy");

        Create
            .Column("accepted_with_comments")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
        
        Execute.Sql(
            """
            UPDATE review.task_for_reviews AS t
            SET accepted_with_comments = t.state = 5;
            """,
            "Set has_concrete_reviewer by strategy");
        
        Execute.Sql(
            """
            UPDATE review.task_for_reviews
            SET state = 4
            WHERE state = 5;
            """,
            "Change state");
    }
}