using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_06_08_0)]
public sealed class ExcludeTeammates : Migration
{
    public override void Up()
    {
        Create
            .Column("leave_until")
            .OnTable("teammates")
            .InSchema("connector")
            .AsDateTimeOffset().Nullable();
    }

    public override void Down()
    {
        Delete
            .Column("leave_until")
            .FromTable("teammates")
            .InSchema("connector");
    }
}