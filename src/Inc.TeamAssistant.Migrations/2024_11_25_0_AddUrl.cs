using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_11_25_0)]

public sealed class AddUrl  : Migration
{
    public override void Up()
    {
        Create
            .Column("url")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsString(2000)
            .Nullable();
        
        Execute.Sql(
            """
            UPDATE appraiser.stories
            SET url = links->>0;
            """,
            "Add single url to story");
        
        Delete
            .Column("links")
            .FromTable("stories")
            .InSchema("appraiser");
    }

    public override void Down()
    {
        Create
            .Column("links")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsCustom("jsonb")
            .Nullable();

        Execute.Sql(
            """
            UPDATE appraiser.stories as s
            SET links =
                CASE
                    WHEN s.url IS NULL
                        THEN '[]'::JSONB
                    ELSE to_jsonb(ARRAY[s.url])
                END;
            """, 
            "Move url to links");

        Alter
            .Column("links")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsCustom("jsonb")
            .NotNullable();
        
        Delete
            .Column("url")
            .FromTable("stories")
            .InSchema("appraiser");
    }
}