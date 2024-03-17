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

        Alter
            .Column("language_id")
            .OnTable("persons")
            .InSchema("connector")
            .AsString().Nullable();
    }

    public override void Down()
    {
        Alter
            .Column("language_id")
            .OnTable("persons")
            .InSchema("connector")
            .AsString().NotNullable();
        
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