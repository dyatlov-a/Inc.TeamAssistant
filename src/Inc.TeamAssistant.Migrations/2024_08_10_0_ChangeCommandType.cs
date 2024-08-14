using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_08_10_0)]
public sealed class ChangeCommandType : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            UPDATE connector.bot_command_stages
            SET value = 3
            WHERE dialog_message_id = 'CheckIn_AddLocation';
            
            UPDATE connector.bot_command_stages
            SET value = 2
            WHERE dialog_message_id = 'Connector_SelectTeam';
            
            UPDATE connector.bot_command_stages
            SET value = 1
            WHERE dialog_message_id IN ('Appraiser_EnterStoryName', 'Reviewer_EnterRequestForReview', 'Connector_EnterTeamName');
            """,
            "Change command type");
    }

    public override void Down()
    {
    }
}