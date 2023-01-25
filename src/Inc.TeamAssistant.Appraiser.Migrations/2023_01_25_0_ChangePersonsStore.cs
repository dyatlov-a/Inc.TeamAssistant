using FluentMigrator;

namespace Inc.TeamAssistant.Appraiser.Migrations;

[Migration(2023_01_25_0)]
public class ChangePersonsStore : Migration
{
    public override void Up()
    {
        Delete
            .PrimaryKey("task_for_reviews__players__fk__owner_id__id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Delete
            .PrimaryKey("task_for_reviews__players__fk__reviewer_id__id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Create
            .Column("new_owner_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt64().Nullable();
        
        Create
            .Column("new_reviewer_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .AsInt64().Nullable();
        
        Execute.Sql(@"
            UPDATE review.task_for_reviews AS tt
            SET new_owner_id = p.person__id
            FROM review.task_for_reviews AS st
            JOIN review.players p ON p.id = st.owner_id
            WHERE tt.id = st.id;");
        
        Execute.Sql(@"
            UPDATE review.task_for_reviews AS tt
            SET new_reviewer_id = p.person__id
            FROM review.task_for_reviews AS st
            JOIN review.players p ON p.id = st.reviewer_id
            WHERE tt.id = st.id;");

        Alter
            .Table("task_for_reviews")
            .InSchema("review")
            .AlterColumn("new_owner_id")
            .AsInt64().NotNullable();
        
        Alter
            .Table("task_for_reviews")
            .InSchema("review")
            .AlterColumn("new_reviewer_id")
            .AsInt64().NotNullable();
        
        Delete
            .Column("owner_id")
            .Column("reviewer_id")
            .FromTable("task_for_reviews")
            .InSchema("review");
        
        Rename
            .Column("new_owner_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .To("owner_id");
        
        Rename
            .Column("new_reviewer_id")
            .OnTable("task_for_reviews")
            .InSchema("review")
            .To("reviewer_id");

        Create
            .Table("persons")
            .InSchema("review")

            .WithColumn("id")
            .AsInt64().NotNullable()
            .PrimaryKey("persons__pk__id")
            
            .WithColumn("language_id")
            .AsString().NotNullable()
            
            .WithColumn("first_name")
            .AsString().NotNullable()
            
            .WithColumn("last_name")
            .AsString().Nullable()
            
            .WithColumn("username")
            .AsString().Nullable();
        
        Execute.Sql(@"
            INSERT INTO review.persons (id, language_id, first_name, last_name, username)
            SELECT DISTINCT person__id, person__language_id, person__first_name, person__last_name, person__username
            FROM review.players;");

        Delete
            .PrimaryKey("players__pk__id")
            .FromTable("players")
            .InSchema("review");
            
        Rename
            .Column("person__id")
            .OnTable("players")
            .InSchema("review")
            .To("person_id");
            
        Delete
            .Column("id")
            .Column("person__language_id")
            .Column("person__first_name")
            .Column("person__last_name")
            .Column("person__username")
            .FromTable("players")
            .InSchema("review");
        
        Create
            .PrimaryKey("players__pk__id")
            .OnTable("players")
            .WithSchema("review")
            .Columns("team_id", "person_id");
        
        Create
            .ForeignKey("players__persons__fk__person_id__id")
            .FromTable("players")
            .InSchema("review")
            .ForeignColumn("person_id")
            .ToTable("persons")
            .InSchema("review")
            .PrimaryColumn("id");
        
        Create
            .ForeignKey("task_for_reviews__persons__fk__owner_id__id")
            .FromTable("task_for_reviews")
            .InSchema("review")
            .ForeignColumn("owner_id")
            .ToTable("persons")
            .InSchema("review")
            .PrimaryColumn("id");
        
        Create
            .ForeignKey("task_for_reviews__persons__fk__reviewer_id__id")
            .FromTable("task_for_reviews")
            .InSchema("review")
            .ForeignColumn("reviewer_id")
            .ToTable("persons")
            .InSchema("review")
            .PrimaryColumn("id");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}