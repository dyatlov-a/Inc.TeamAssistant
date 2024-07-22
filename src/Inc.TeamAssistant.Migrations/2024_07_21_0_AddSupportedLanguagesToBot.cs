using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_07_21_0)]
public sealed class AddSupportedLanguagesToBot : Migration
{
    public override void Up()
    {
        Create
            .Column("supported_languages")
            .OnTable("bots")
            .InSchema("connector")

            .AsCustom("jsonb").NotNullable()
            .SetExistingRowsTo("[\"en\", \"ru\"]");
    }

    public override void Down()
    {
        Delete
            .Column("supported_languages")
            .FromTable("bots")
            .InSchema("connector");
    }
}