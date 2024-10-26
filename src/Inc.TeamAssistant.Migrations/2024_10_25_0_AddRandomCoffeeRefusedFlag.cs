using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_10_25_0)]

public sealed class AddRandomCoffeeRefusedFlag : Migration 
{
    public override void Up()
    {
        Create
            .Column("refused")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_commands(id, value, help_message_id, scopes)
            VALUES
                ('d6097f8b-de33-412c-a064-68069d458776', '/refuse', 'RandomCoffee_RefuseHelp', '[2]'::jsonb)
            """,
            "Insert new random coffee command");
        Execute.Sql(
            """
            INSERT INTO connector.command_packs(feature_id, bot_command_id)
            VALUES
                ('39195e70-b83a-42b3-88e5-dbbf6789a3c8', 'd6097f8b-de33-412c-a064-68069d458776')
            """,
            "Insert new random coffee command");
    }

    public override void Down()
    {
        Delete
            .Column("refused")
            .FromTable("entries")
            .InSchema("random_coffee");
        
        Execute.Sql(
            """
            DELETE FROM connector.command_packs
            WHERE feature_id = '39195e70-b83a-42b3-88e5-dbbf6789a3c8'
                AND bot_command_id = 'd6097f8b-de33-412c-a064-68069d458776';
            """,
            "Clear new random coffee command");
        
        Execute.Sql(
            """
            DELETE FROM connector.bot_commands
                WHERE id = 'd6097f8b-de33-412c-a064-68069d458776';
            """,
            "Clear new random coffee command");
    }
}