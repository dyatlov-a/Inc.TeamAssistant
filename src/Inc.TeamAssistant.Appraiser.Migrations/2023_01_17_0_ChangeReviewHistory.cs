using FluentMigrator;

namespace Inc.TeamAssistant.Appraiser.Migrations;

[Migration(2023_01_17_0)]
public class ChangeReviewHistory : Migration
{
    public override void Up()
    {
        Delete
            .Column("last_reviewer_id")
            .FromTable("players")
            .InSchema("review");

        Create
            .Column("created")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsDateTimeOffset()
            .NotNullable()
            .SetExistingRowsTo(DateTimeOffset.UtcNow);

        Create
            .Column("team_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsGuid()
            .Nullable();
        
        Execute.Sql(@"
            UPDATE review.task_for_reviews AS tt
            SET tt.team_id = p.team_id
            FROM review.task_for_reviews AS st
            JOIN review.players p ON p.id = st.owner_id
            WHERE tt.id = st.id;");

        Alter
            .Column("team_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsGuid()
            .NotNullable();

        Create
            .ForeignKey("task_for_reviews__teams__fk__team_id__id")
            .FromTable("task_for_reviews")
            .InSchema("review")
            .ForeignColumn("team_id")
            .ToTable("teams")
            .InSchema("review")
            .PrimaryColumn("id");
    }

    public override void Down()
    {
        Delete
            .ForeignKey("task_for_reviews__teams__fk__team_id__id")
            .OnTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("team_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .Column("created")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Create
            .Column("last_reviewer_id")
            .OnTable("players")
            .InSchema("review")
            .AsInt64()
            .Nullable();
    }
}