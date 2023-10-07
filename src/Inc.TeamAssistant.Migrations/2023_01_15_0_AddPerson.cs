using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2023_01_15_0)]
public sealed class AddPerson : Migration
{
    public override void Up()
    {
        Rename
            .Column("user_id")
            .OnTable("players")
            .InSchema("review")
            .To("person__id");
        
        Rename
            .Column("name")
            .OnTable("players")
            .InSchema("review")
            .To("person__first_name");
        
        Rename
            .Column("language_id")
            .OnTable("players")
            .InSchema("review")
            .To("person__language_id");
        
        Rename
            .Column("login")
            .OnTable("players")
            .InSchema("review")
            .To("person__username");

        Create
            .Column("person__last_name")
            .OnTable("players")
            .InSchema("review")
            .AsString()
            .Nullable();

        Create
            .Column("next_reviewer_type")
            .OnTable("teams")
            .InSchema("review")
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(1);
    }

    public override void Down()
    {
        Delete
            .Column("next_reviewer_type")
            .FromTable("teams")
            .InSchema("review");
        
        Delete
            .Column("person__last_name")
            .FromTable("players")
            .InSchema("review");
        
        Rename
            .Column("person__username")
            .OnTable("players")
            .InSchema("review")
            .To("login");
        
        Rename
            .Column("person__language_id")
            .OnTable("players")
            .InSchema("review")
            .To("language_id");
        
        Rename
            .Column("person__first_name")
            .OnTable("players")
            .InSchema("review")
            .To("name");
        
        Rename
            .Column("person__id")
            .OnTable("players")
            .InSchema("review")
            .To("user_id");
    }
}