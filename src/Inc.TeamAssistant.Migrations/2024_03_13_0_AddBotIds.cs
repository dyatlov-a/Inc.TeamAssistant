using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_03_13_0)]
public sealed class AddBotIds : Migration
{
    public override void Up()
    {
        Create
            .Column("bot_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsGuid().NotNullable().SetExistingRowsTo(Guid.Empty);
        
        Create
            .Column("bot_id")
            .OnTable("maps")
            .InSchema("maps")
            .AsGuid().NotNullable().SetExistingRowsTo(Guid.Empty);
        
        Create
            .Column("bot_id")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsGuid().NotNullable().SetExistingRowsTo(Guid.Empty);

        Create
            .Table("client_languages")
            .InSchema("connector")

            .WithColumn("person_id")
            .AsInt64().NotNullable()
            .PrimaryKey("client_languages__pk__person_id")

            .WithColumn("language_id")
            .AsString().NotNullable();

        Execute.Sql("""
            INSERT INTO connector.client_languages (person_id, language_id)
            SELECT id, language_id
            FROM connector.persons;
            """,
            "Migrate languages");

        Delete
            .Column("language_id")
            .FromTable("persons")
            .InSchema("connector");
    }

    public override void Down()
    {
        Create
            .Column("language_id")
            .OnTable("persons")
            .InSchema("connector")
            .AsString().NotNullable()
            .SetExistingRowsTo(string.Empty);
        
        Delete
            .Table("client_languages")
            .InSchema("connector");
        
        Delete
            .Column("bot_id")
            .FromTable("stories")
            .InSchema("appraiser");
        
        Delete
            .Column("bot_id")
            .FromTable("maps")
            .InSchema("maps");
        
        Delete
            .Column("bot_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
    }
}