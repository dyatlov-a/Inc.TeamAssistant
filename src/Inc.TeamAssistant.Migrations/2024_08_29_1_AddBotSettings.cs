using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_08_29_1)]
public sealed class AddBotSettings : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            UPDATE connector.features
            SET properties = '["nextReviewerStrategy", "waitingNotificationInterval", "inProgressNotificationInterval"]'::jsonb
            WHERE name = 'Reviewer';
            """,
            "Add settings for Reviewer");
        
        Execute.Sql(
            """
            UPDATE connector.features
            SET properties = '["roundInterval", "votingInterval"]'::jsonb
            WHERE name = 'RandomCoffee';
            """,
            "Add settings for RandomCoffee");
    }

    public override void Down()
    {
    }
}