using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_04_21_0)]
public sealed class AddSecondRoundToReview : Migration
{
    public override void Up()
    {
        Create
            .Column("can_finalize")
            .OnTable("teammates")
            .InSchema("connector")
            .AsBoolean().NotNullable()
            .SetExistingRowsTo(false);

        Create
            .Column("first_reviewer_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt64().Nullable();
        
        Create
            .Column("first_reviewer_message_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt32().Nullable();
        
        Create
            .Column("second_reviewer_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt64().Nullable();
        
        Execute.Sql(
            "UPDATE review.task_for_reviews SET first_reviewer_id = reviewer_id WHERE first_reviewer_id IS NULL",
            "Set first_reviewer_id for task_for_reviews");
        
        Execute.Sql(
            "UPDATE review.task_for_reviews SET state = state + 1 WHERE state > 3;",
            "Change state code for AcceptWithComments and Accept");
    }

    public override void Down()
    {
        Execute.Sql(
            "UPDATE review.task_for_reviews SET state = state - 1 WHERE state > 3;",
            "Rollback state code for AcceptWithComments and Accept");
        
        Delete
            .Column("second_reviewer_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("first_reviewer_message_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("first_reviewer_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("can_finalize")
            .FromTable("teammates")
            .InSchema("connector");
    }
}