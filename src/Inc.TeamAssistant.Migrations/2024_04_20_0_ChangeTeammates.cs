using System.Data;
using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_04_20_0)]
public sealed class ChangeTeammates : Migration
{
    public override void Up()
    {
        Delete
            .ForeignKey("teammates__fk__person_id__id")
            .OnTable("teammates")
            .InSchema("connector");

        Create
            .ForeignKey("teammates__fk__person_id__id")
            .FromTable("teammates")
            .InSchema("connector")
            .ForeignColumn("person_id")
            .ToTable("persons")
            .InSchema("connector")
            .PrimaryColumn("id")
            .OnDelete(Rule.Cascade);
        
        Delete
            .ForeignKey("teammates__fk__team_id__id")
            .OnTable("teammates")
            .InSchema("connector");
        
        Create
            .ForeignKey("teammates__fk__team_id__id")
            .FromTable("teammates")
            .InSchema("connector")
            .ForeignColumn("team_id")
            .ToTable("teams")
            .InSchema("connector")
            .PrimaryColumn("id")
            .OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
    }
}