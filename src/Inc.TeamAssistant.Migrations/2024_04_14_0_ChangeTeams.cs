using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_04_14_0)]
public sealed class ChangeTeams : Migration
{
    public override void Up()
    {
        Execute.Sql(
            "UPDATE connector.teams SET name = replace(name, ' ', '_');",
            "Remove spaces from team names");

        Create
            .Column("original_reviewer_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt64().Nullable();
    }

    public override void Down()
    {
        Delete
            .Column("original_reviewer_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}