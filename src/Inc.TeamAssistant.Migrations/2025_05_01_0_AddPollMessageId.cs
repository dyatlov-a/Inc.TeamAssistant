using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_05_01_0)]
public sealed class AddPollMessageId : Migration
{
    public override void Up()
    {
        Create
            .Column("poll")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsCustom("jsonb")
            .Nullable();
        
        Delete
            .Column("poll_id")
            .FromTable("entries")
            .InSchema("random_coffee");
    }

    public override void Down()
    {
        Create
            .Column("poll_id")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsString(250)
            .Nullable();
        
        Delete
            .Column("poll")
            .FromTable("entries")
            .InSchema("random_coffee");
    }
}