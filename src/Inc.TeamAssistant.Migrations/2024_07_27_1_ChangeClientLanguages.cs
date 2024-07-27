using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_07_27_1)]
public sealed class ChangeClientLanguages : Migration
{
    public override void Up()
    {
        Delete
            .PrimaryKey("client_languages__pk__person_id")
            .FromTable("client_languages")
            .InSchema("connector");
        
        Create
            .PrimaryKey("client_languages__pk__person_id__language_id")
            .OnTable("client_languages")
            .WithSchema("connector")
            .Columns("person_id", "language_id");
        
        Create
            .Column("last_use")
            .OnTable("client_languages")
            .InSchema("connector")
            
            .AsDateTimeOffset()
            .NotNullable()
            .SetExistingRowsTo(DateTimeOffset.UtcNow);
    }

    public override void Down()
    {
        Execute.Sql(
            "DELETE FROM connector.client_languages;",
            "Delete client languages");
        
        Delete
            .PrimaryKey("client_languages__pk__person_id__language_id")
            .FromTable("client_languages")
            .InSchema("connector");
        
        Create
            .PrimaryKey("client_languages__pk__person_id")
            .OnTable("client_languages")
            .WithSchema("connector")
            .Columns("person_id");
        
        Delete
            .Column("last_use")
            .FromTable("client_languages")
            .InSchema("connector");
    }
}