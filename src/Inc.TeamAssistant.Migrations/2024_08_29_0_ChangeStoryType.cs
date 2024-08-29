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
    }

    public override void Down()
    {
    }
}