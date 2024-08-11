using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_08_09_0)]

public sealed class AddTotalValue : Migration 
{
    public override void Up()
    {
        Create
            .Column("total_value")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsInt32().Nullable();
        
        Delete
            .Column("accepted")
            .FromTable("stories")
            .InSchema("appraiser");
        
        Execute.Sql(@"UPDATE appraiser.stories AS s
                SET total_value = (SELECT MAX(se.value) AS value
                    FROM appraiser.story_for_estimates AS se
                    WHERE s.id = se.story_id
                    GROUP BY se.story_id)",
            "calculating and setting total value for all stories");
    }

    public override void Down()
    {
        Delete
            .Column("total_value")
            .FromTable("stories")
            .InSchema("appraiser");

        Create
            .Column("accepted")
            .OnTable("stories")
            .InSchema("appraiser")
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(true);
        
    }
}