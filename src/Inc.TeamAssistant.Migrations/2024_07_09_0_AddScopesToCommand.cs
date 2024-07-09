using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_07_09_0)]
public sealed class AddScopesToCommand : Migration
{
    public override void Up()
    {
        Create
            .Column("scopes")
            .OnTable("bot_commands")
            .InSchema("connector")
            
            .AsCustom("jsonb")
            .NotNullable()
            .SetExistingRowsTo("[]");

        Execute.Sql(
            """
            UPDATE connector.bot_commands
            SET scopes = '["Default", "AllGroupChats"]'::jsonb
            WHERE value IN (
                '/leave_team',
                '/cancel',
                '/add',
                '/move_to_sp',
                '/move_to_tshirts',
                '/need_review',
                '/change_to_round_robin',
                '/change_to_random',
                '/remove_team',
                '/help');

            UPDATE connector.bot_commands
            SET scopes = '["AllGroupChats"]'::jsonb
            WHERE value IN (
                '/location',
                '/invite',
                '/new_team');
            """);
    }

    public override void Down()
    {
        Delete
            .Column("scopes")
            .FromTable("bot_commands")
            .InSchema("connector");
    }
}