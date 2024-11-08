using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_11_07_0)]

public sealed class AddRoundsCount  : Migration
{
    public override void Up()
    {
        Create
            .Column("rounds_count")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(1);
    }

    public override void Down()
    {
        Delete
            .Column("rounds_count")
            .FromTable("stories")
            .InSchema("appraiser");
    }
}