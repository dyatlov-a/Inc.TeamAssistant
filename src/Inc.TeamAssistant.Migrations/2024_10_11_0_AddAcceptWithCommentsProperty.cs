using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_10_11_0)]

public sealed class AddAcceptWithCommentsProperty : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            UPDATE connector.features
            SET properties = '["nextReviewerStrategy", "waitingNotificationInterval", "inProgressNotificationInterval", "acceptWithComments"]'::jsonb
            WHERE name = 'Reviewer';
            """,
            "Add settings for Reviewer");
    }

    public override void Down()
    {
        Execute.Sql(
            """
            UPDATE connector.features
            SET properties = '["nextReviewerStrategy", "waitingNotificationInterval", "inProgressNotificationInterval"]'::jsonb
            WHERE name = 'Reviewer';
            """,
            "Remove settings for Reviewer");
    }
}