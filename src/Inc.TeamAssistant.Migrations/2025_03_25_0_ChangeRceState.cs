using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_03_25_0)]
public sealed class ChangeRceState : Migration
{
    public override void Up()
    {
        Delete
            .Column("refused")
            .FromTable("entries")
            .InSchema("random_coffee");
    }

    public override void Down()
    {
        Create
            .Column("refused")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
    }
}