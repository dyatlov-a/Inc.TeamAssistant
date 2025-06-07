using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_06_01_0)]
public sealed class CreateBacklog : Migration
{
    public override void Up()
    {
        Create
            .Table("action_items")
            .InSchema("retro")
            
            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("action_items__pk__id")
            
            .WithColumn("retro_item_id")
            .AsGuid().NotNullable()
            .ForeignKey("action_items__fk__retro_item_id", "retro", "retro_items", "id")
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("text")
            .AsString().Nullable()
            
            .WithColumn("state")
            .AsInt32().NotNullable()
            
            .WithColumn("modified")
            .AsDateTimeOffset().Nullable();
    }

    public override void Down()
    {
        Delete
            .Table("action_items")
            .InSchema("retro");
    }
}