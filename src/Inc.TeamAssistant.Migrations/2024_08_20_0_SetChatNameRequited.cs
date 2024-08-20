using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_08_20_0)]
public sealed class SetChatNameRequited : Migration
{
    public override void Up()
    {
        Alter
            .Column("name")
            .OnTable("maps")
            .InSchema("maps")
            .AsString(128).NotNullable();
        
        Alter
            .Column("name")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsString(128).NotNullable();
    }

    public override void Down()
    {
        Alter
            .Column("name")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsString(128).Nullable();
        
        Alter
            .Column("name")
            .OnTable("maps")
            .InSchema("maps")
            .AsString(128).Nullable();
    }
}