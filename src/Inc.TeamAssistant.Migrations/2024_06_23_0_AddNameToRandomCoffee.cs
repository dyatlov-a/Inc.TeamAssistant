using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_06_23_0)]
public sealed class AddNameToRandomCoffee : Migration
{
    public override void Up()
    {
        Create
            .Column("name")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsString(128).Nullable();
    }

    public override void Down()
    {
        Delete
            .Column("name")
            .FromTable("entries")
            .InSchema("random_coffee");
    }
}