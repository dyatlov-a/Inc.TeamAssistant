using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_08_29_0)]
public sealed class ChangeStoryType : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            UPDATE connector.teams
            SET properties = properties || '{"storyType": "Fibonacci"}'
            WHERE properties ->> 'storyType' = 'Scrum';
            """,
            "Change Teams StoryType Scrum -> Fibonacci");
        
        Execute.Sql(
            """
            UPDATE connector.teams
            SET properties = properties || '{"storyType": "TShirt"}'
            WHERE properties ->> 'storyType' = 'Kanban';
            """,
            "Change Teams StoryType Kanban -> TShirt");
        
        Execute.Sql(
            """
            UPDATE connector.bots
            SET properties = properties || '{"storyType": "Fibonacci"}'
            WHERE properties ->> 'storyType' = 'Scrum';
            """,
            "Change Bots StoryType Scrum -> Fibonacci");
        
        Execute.Sql(
            """
            UPDATE connector.bots
            SET properties = properties || '{"storyType": "TShirt"}'
            WHERE properties ->> 'storyType' = 'Kanban';
            """,
            "Change Bots StoryType Kanban -> TShirt");

        Execute.Sql(
            """
            UPDATE connector.bot_commands
            SET
            	value = '/move_to_fibonacci',
            	help_message_id = 'Appraiser_MoveToFibonacciHelp'
            WHERE value = '/move_to_sp';
            """,
            "Change bot command /move_to_sp -> /move_to_fibonacci");
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_commands
            (id, value, help_message_id, scopes)
            VALUES ('91169669-bf86-4d66-b721-7e4d9878e5be', '/move_to_power_of_two', 'Appraiser_MoveToPowerOfTwoHelp', '[1,2]'::jsonb);
            """,
            "Add command for moving to PowerOfTwo");
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_command_stages
            (id, bot_command_id, value, dialog_message_id, "position")
            VALUES ('003caff2-1b28-48b6-966b-de1441b7ecdc', '91169669-bf86-4d66-b721-7e4d9878e5be', 2, 'Connector_SelectTeam', 1);
            """,
            "Add stage for command moving to PowerOfTwo");

        Execute.Sql(
            """
            INSERT INTO connector.command_packs
            (feature_id, bot_command_id)
            VALUES ('5a7334e6-8076-4fc1-89e9-5139b8135947', '91169669-bf86-4d66-b721-7e4d9878e5be');
            """,
            "Add command to command packs");
    }

    public override void Down()
    {
    }
}