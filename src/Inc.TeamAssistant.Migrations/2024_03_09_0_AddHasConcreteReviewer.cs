using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_03_09_0)]
public sealed class AddHasConcreteReviewer : Migration
{
    public override void Up()
    {
        Create
            .Column("has_concrete_reviewer")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsBoolean().NotNullable().SetExistingRowsTo(false);
    }

    public override void Down()
    {
        Delete
            .Column("has_concrete_reviewer")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}