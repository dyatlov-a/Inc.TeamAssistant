using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2023_01_11_0)]
public sealed class AddTaskDecline : Migration
{
    public override void Up()
    {
        Delete
            .Column("is_active")
            .FromTable("task_for_reviews")
            .InSchema("review");

        Create
            .Column("state")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(3);
        
        Create
            .Column("accept_date")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsDateTimeOffset()
            .Nullable();
    }

    public override void Down()
    {
        Delete
            .Column("accept_date")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("state")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Create
            .Column("is_active")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
    }
}