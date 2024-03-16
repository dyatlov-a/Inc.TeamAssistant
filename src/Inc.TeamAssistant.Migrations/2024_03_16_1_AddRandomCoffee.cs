using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_03_16_1)]
public sealed class AddRandomCoffee : Migration
{
    public override void Up()
    {
        Create
            .Table("entries")
            .InSchema("random_coffee")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("entries__pk__id")
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("bot_id")
            .AsGuid().NotNullable()
            
            .WithColumn("chat_id")
            .AsInt64().NotNullable()
            
            .WithColumn("next_round")
            .AsDate().NotNullable()
            
            .WithColumn("state")
            .AsInt32().NotNullable()
            
            .WithColumn("poll_id")
            .AsString(250).Nullable()
            
            .WithColumn("participant_ids")
            .AsCustom("jsonb").NotNullable();

        Create
            .Table("history")
            .InSchema("random_coffee")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("history__pk__id")

            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("random_coffee_entry_id")
            .AsGuid().NotNullable()
            .ForeignKey(
                foreignKeyName: "history__entries__fk__random_coffee_entry_id__id",
                primaryTableSchema: "random_coffee",
                primaryTableName: "entries",
                primaryColumnName: "id")
            
            .WithColumn("pairs")
            .AsCustom("jsonb").NotNullable()
            
            .WithColumn("excluded_person_id")
            .AsInt64().Nullable();
    }

    public override void Down()
    {
        Delete
            .Table("history")
            .InSchema("random_coffee");
        
        Delete
            .Table("entries")
            .InSchema("random_coffee");
    }
}