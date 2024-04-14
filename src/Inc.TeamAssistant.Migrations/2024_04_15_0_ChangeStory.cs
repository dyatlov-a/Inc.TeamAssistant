using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_04_15_0)]
public sealed class ChangeStory : Migration
{
    public override void Up()
    {
        Create
            .Column("is_finished")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsBoolean().NotNullable().SetExistingRowsTo(false);
    }

    public override void Down()
    {
        Delete
            .Column("is_finished")
            .FromTable("stories")
            .InSchema("appraiser");
    }
}