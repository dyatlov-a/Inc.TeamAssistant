using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_01_11_0)]
public sealed class ChangeReviewerTables : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            INSERT INTO connector.persons(id, name, language_id, username)
            SELECT id, first_name, language_id, username
            FROM review.persons
            ON CONFLICT (id) DO NOTHING;",
            "Migrate persons from review to connector");
        
        Execute.Sql(@"
            INSERT INTO connector.teams(id, bot_id, chat_id, name, properties)
            SELECT id, 'e5b2d82b-3912-4d94-acb8-c6e603622a95', chat_id, name, '{}'::jsonb
            FROM review.teams
            ON CONFLICT (id) DO NOTHING;",
            "Migrate teams from review to connector");
        
        Execute.Sql(@"
            INSERT INTO connector.teammates(team_id, person_id)
            SELECT team_id, person_id
            FROM review.players
            ON CONFLICT (team_id, person_id) DO NOTHING;",
            "Migrate players from review to connector");
        
        Delete
            .ForeignKey("task_for_reviews__persons__fk__owner_id__id")
            .OnTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .ForeignKey("task_for_reviews__persons__fk__reviewer_id__id")
            .OnTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .ForeignKey("task_for_reviews__teams__fk__team_id__id")
            .OnTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Table("players")
            .InSchema("review");
        
        Delete
            .Table("teams")
            .InSchema("review");
        
        Delete
            .Table("persons")
            .InSchema("review");

        Create
            .Column("strategy")
            .OnTable("task_for_reviews")
            .InSchema("review")

            .AsInt32().NotNullable().SetExistingRowsTo(1);

        Alter
            .Column("description")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsString(2000)
            .NotNullable();
    }

    public override void Down()
    {
    }
}