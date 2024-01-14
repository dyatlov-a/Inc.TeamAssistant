using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_01_10_1)]
public sealed class CreateAppraiserTables : Migration
{
    public override void Up()
    {
        Create
            .Table("stories")
            .InSchema("appraiser")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("stories__pk__id")
            
            .WithColumn("story_type")
            .AsInt32().NotNullable()
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("team_id")
            .AsGuid().NotNullable()
            
            .WithColumn("chat_id")
            .AsInt64().NotNullable()
            
            .WithColumn("moderator_id")
            .AsInt64().NotNullable()
            
            .WithColumn("language_id")
            .AsString().NotNullable()
            
            .WithColumn("title")
            .AsString().NotNullable()
            
            .WithColumn("external_id")
            .AsInt32().Nullable()
            
            .WithColumn("links")
            .AsCustom("jsonb").NotNullable();

        Create
            .Table("story_for_estimates")
            .InSchema("appraiser")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("story_for_estimates__pk__id")

            .WithColumn("story_id")
            .AsGuid().NotNullable()
            .ForeignKey(
                foreignKeyName: "story_for_estimates__fk__story_id__id",
                primaryColumnName: "id",
                primaryTableName: "stories",
                primaryTableSchema: "appraiser")

            .WithColumn("participant_id")
            .AsInt64().NotNullable()

            .WithColumn("participant_display_name")
            .AsString().NotNullable()

            .WithColumn("value")
            .AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("story_for_estimates")
            .InSchema("appraiser");
        
        Delete
            .Table("stories")
            .InSchema("appraiser");
    }
}