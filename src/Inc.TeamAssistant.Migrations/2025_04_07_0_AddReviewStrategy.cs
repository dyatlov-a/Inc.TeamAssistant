using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_04_07_0)]
public sealed class AddReviewStrategy : Migration
{
    public override void Up()
    {
        Execute.Sql(
            "UPDATE review.task_for_reviews SET strategy = 11 WHERE strategy = 2;",
            "Change review strategy for random");
        
        Execute.Sql(
            "UPDATE review.task_for_reviews SET strategy = 100 WHERE strategy = 3;",
            "Change review strategy for target");

        Create
            .Column("original_reviewer_message_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt32().Nullable();
        
        Execute.Sql(
            """
            UPDATE review.task_for_reviews
            SET original_reviewer_message_id = reviewer_message_id
            WHERE original_reviewer_message_id IS NULL;
            """,
            "Set original_reviewer_message_id for all tasks");
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_commands(id, value, help_message_id, scopes)
            VALUES
                ('42ddafda-a478-4f6e-b626-4855912813e7', '/change_to_team_round_robin', 'Reviewer_ChangeToRoundRobinForTeamHelp', '[1, 2]'::jsonb)
            """,
            "Insert new reviewer commands");
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_command_stages(id, bot_command_id, value, dialog_message_id, position)
            VALUES
            	('cb4d6b85-69a8-405c-918b-4277da537c94', '42ddafda-a478-4f6e-b626-4855912813e7', 2, 'Connector_SelectTeam', 1);
            """,
            "Insert new reviewer bot_command_stages");
        
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