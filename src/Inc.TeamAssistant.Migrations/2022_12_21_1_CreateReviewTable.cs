using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2022_12_21_1)]
public sealed class CreateReviewTable : Migration
{
    public override void Up()
    {
        Create
            .Table("teams")
            .InSchema("review")

            .WithColumn("id")
            .AsGuid()
            .NotNullable()
            .PrimaryKey("teams__pk__id")

            .WithColumn("chat_id")
            .AsInt64()
            .NotNullable()

            .WithColumn("name")
            .AsString()
            .NotNullable();

        Create
            .Table("players")
            .InSchema("review")

            .WithColumn("id")
            .AsGuid()
            .NotNullable()
            .PrimaryKey("players__pk__id")

            .WithColumn("user_id")
            .AsInt64()
            .NotNullable()

            .WithColumn("team_id")
            .AsGuid()
            .NotNullable()
            .ForeignKey(
                foreignKeyName: "players__teams__fk__team_id__id",
                primaryTableSchema: "review",
                primaryTableName: "teams",
                primaryColumnName: "id")

            .WithColumn("name")
            .AsString()
            .NotNullable()

            .WithColumn("last_reviewer_id")
            .AsInt64()
            .Nullable()

            .WithColumn("language_id")
            .AsString()
            .NotNullable()

            .WithColumn("login")
            .AsString()
            .Nullable();

        Create
            .Table("task_for_reviews")
            .InSchema("review")

            .WithColumn("id")
            .AsGuid()
            .NotNullable()
            .PrimaryKey("task_for_reviews__pk__id")

            .WithColumn("owner_id")
            .AsGuid()
            .NotNullable()
            .ForeignKey(
                foreignKeyName: "task_for_reviews__players__fk__owner_id__id",
                primaryTableSchema: "review",
                primaryTableName: "players",
                primaryColumnName: "id")

            .WithColumn("reviewer_id")
            .AsGuid()
            .NotNullable()
            .ForeignKey(
                foreignKeyName: "task_for_reviews__players__fk__reviewer_id__id",
                primaryTableSchema: "review",
                primaryTableName: "players",
                primaryColumnName: "id")

            .WithColumn("description")
            .AsString()
            .NotNullable()

            .WithColumn("is_active")
            .AsBoolean()
            .NotNullable()

            .WithColumn("next_notification")
            .AsDateTimeOffset()
            .NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("task_for_reviews")
            .InSchema("review");

        Delete
            .Table("players")
            .InSchema("review");

        Delete
            .Table("teams")
            .InSchema("review");
    }
}