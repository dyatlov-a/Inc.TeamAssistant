using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_10_03_0)]

public sealed class AddAcceptWithCommentsFlag : Migration
{
    public override void Up()
    {
        Create
            .Column("accepted_with_comments")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsBoolean().Nullable();
    }

    public override void Down()
    {
        Delete
            .Column("accepted_with_comments")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}