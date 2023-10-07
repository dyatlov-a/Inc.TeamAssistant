using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2023_01_02_1)]
public sealed class CreateHolidaysTable : Migration
{
    public override void Up()
    {
        Create
            .Table("holidays")
            .InSchema("generic")

            .WithColumn("date")
            .AsDate().NotNullable()
            .PrimaryKey("generic__pk__date")

            .WithColumn("type")
            .AsInt32().NotNullable();

        Insert
            .IntoTable("holidays")
            .InSchema("generic")

            .Row(new { date = new DateTime(2023, 01, 02), type = 1 })
            .Row(new { date = new DateTime(2023, 01, 03), type = 1 })
            .Row(new { date = new DateTime(2023, 01, 04), type = 1 })
            .Row(new { date = new DateTime(2023, 01, 05), type = 1 })
            .Row(new { date = new DateTime(2023, 01, 06), type = 1 })
            .Row(new { date = new DateTime(2023, 02, 23), type = 1 })
            .Row(new { date = new DateTime(2023, 02, 24), type = 1 })
            .Row(new { date = new DateTime(2023, 03, 08), type = 1 })
            .Row(new { date = new DateTime(2023, 05, 01), type = 1 })
            .Row(new { date = new DateTime(2023, 05, 08), type = 1 })
            .Row(new { date = new DateTime(2023, 05, 09), type = 1 })
            .Row(new { date = new DateTime(2023, 06, 12), type = 1 })
            .Row(new { date = new DateTime(2023, 11, 06), type = 1 });
    }

    public override void Down()
    {
        Delete
            .Table("holidays")
            .InSchema("generic");
    }
}