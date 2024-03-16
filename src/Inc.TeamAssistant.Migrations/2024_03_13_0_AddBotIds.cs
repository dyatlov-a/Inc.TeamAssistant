using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_03_13_0)]
public sealed class AddBotIds : Migration
{
    public override void Up()
    {
        Create
            .Column("bot_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsGuid().NotNullable().SetExistingRowsTo(Guid.Empty);
        
        Create
            .Column("bot_id")
            .OnTable("maps")
            .InSchema("maps")
            .AsGuid().NotNullable().SetExistingRowsTo(Guid.Empty);
        
        Create
            .Column("bot_id")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsGuid().NotNullable().SetExistingRowsTo(Guid.Empty);
    }

    public override void Down()
    {
        Delete
            .Column("bot_id")
            .FromTable("stories")
            .InSchema("appraiser");
        
        Delete
            .Column("bot_id")
            .FromTable("maps")
            .InSchema("maps");
        
        Delete
            .Column("bot_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}