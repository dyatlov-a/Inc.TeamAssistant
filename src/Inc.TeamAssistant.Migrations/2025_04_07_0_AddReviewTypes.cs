using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_04_07_0)]
public sealed class AddReviewTypes : Migration
{
    public override void Up()
    {
        Execute.Sql(
            "UPDATE reviewer.task_for_reviews SET type = 11 WHERE type = 2;",
            "Change review strategy for random");
        
        Execute.Sql(
            "UPDATE reviewer.task_for_reviews SET type = 100 WHERE type = 3;",
            "Change review strategy for target");
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_commands(id, value, help_message_id, scopes)
            VALUES
                ('42ddafda-a478-4f6e-b626-4855912813e7', '/change_to_team_round_robin', 'Reviewer_ChangeToRoundRobinForTeamHelp', '[1, 2]'::jsonb)
            """,
            "Insert new reviewer commands");
        Execute.Sql(
            """
            INSERT INTO connector.command_packs(feature_id, bot_command_id)
            VALUES
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '42ddafda-a478-4f6e-b626-4855912813e7')
            """,
            "Add new reviewer commands to command pack");
    }

    public override void Down()
    {
    }
}