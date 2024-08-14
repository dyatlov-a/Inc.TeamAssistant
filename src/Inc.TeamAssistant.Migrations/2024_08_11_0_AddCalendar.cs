using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_08_11_0)]
public sealed class AddCalendar : Migration
{
    public override void Up()
    {
        var defaultCalendarId = Guid.Parse("7aedf09b-bb17-4ddd-bfd9-f63b8cfede65");

        Create
            .Column("calendar_id")
            .OnTable("bots")
            .InSchema("connector")
            .AsGuid().NotNullable()
            .SetExistingRowsTo(defaultCalendarId);
        
        Delete
            .Table("holidays")
            .InSchema("generic");

        Create
            .Table("calendars")
            .InSchema("generic")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("generic__pk__id")
            
            .WithColumn("owner_id")
            .AsInt64().NotNullable()
            
            .WithColumn("schedule")
            .AsCustom("jsonb").Nullable()
            
            .WithColumn("weekends")
            .AsCustom("jsonb").NotNullable()
            
            .WithColumn("holidays")
            .AsCustom("jsonb").NotNullable();
        
        Execute.Sql(
            $$"""
            INSERT INTO generic.calendars(
            id, owner_id, schedule, weekends, holidays)
            VALUES (
            	'{{defaultCalendarId}}',
            	272062137,
            	'{"Start":"07:00:00","End":"16:00:00"}'::jsonb,
            	'[6,0]'::jsonb,
            	'{"2024-01-01":1,"2024-01-02":1,"2024-01-03":1,"2024-01-04":1,"2024-01-05":1,"2024-01-08":1,"2024-02-23":1,"2024-03-08":1,"2024-04-27":2,"2024-04-29":1,"2024-04-30":1,"2024-05-01":1,"2024-05-09":1,"2024-05-10":1,"2024-06-12":1,"2024-11-02":2,"2024-11-04":1,"2024-12-28":2,"2024-12-30":1,"2024-12-31":1}'::jsonb);
            """,
            "Add default calendar");

        Rename
            .Column("next_round")
            .OnTable("entries")
            .InSchema("random_coffee")
            .To("old_next_round");
            
        Create
            .Column("next_round")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsDateTimeOffset().Nullable();

        Execute.Sql(
            """
            UPDATE random_coffee.entries
            SET next_round = old_next_round::DATE + created::TIME AT TIME ZONE 'UTC';
            """,
            "Set next_round for random_coffee.entries");

        Alter
            .Column("next_round")
            .OnTable("entries")
            .InSchema("random_coffee")
            .AsDateTimeOffset().NotNullable();
        
        Delete
            .Column("old_next_round")
            .FromTable("entries")
            .InSchema("random_coffee");
    }

    public override void Down()
    {
    }
}