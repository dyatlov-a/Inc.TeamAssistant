using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_03_04_0)]
public sealed class ChangeDialogs : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            UPDATE connector.bot_commands
            SET help_message_id = null
            WHERE value = '/cancel';
            """,
            "Set help_message_id to null for /cancel");
    }

    public override void Down()
    {
        Execute.Sql(
            """
            UPDATE connector.bot_commands
            SET help_message_id = 'Connector_CancelHelp'
            WHERE value = '/cancel';
            """,
            "Recovered help_message_id for /cancel");
    }
}