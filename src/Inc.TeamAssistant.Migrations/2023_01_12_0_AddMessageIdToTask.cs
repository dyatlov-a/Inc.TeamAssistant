using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2023_01_12_0)]
public sealed class AddMessageIdToTask : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            UPDATE review.task_for_reviews
            SET state = state + 1
            WHERE state != 0;");
        
        Create
            .Column("message_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt32()
            .Nullable();

        Create
            .Column("chat_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt64()
            .NotNullable()
            .SetExistingRowsTo(0);
    }

    public override void Down()
    {
        Delete
            .Column("chat_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("message_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Execute.Sql(@"
            UPDATE review.task_for_reviews
            SET state = state - 1
            WHERE state != 0;");
    }
}