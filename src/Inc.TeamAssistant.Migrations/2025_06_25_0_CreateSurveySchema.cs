using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_06_25_0)]
public sealed class CreateSurveySchema : Migration
{
    public override void Up()
    {
        Create
            .Schema("survey");

        Execute.Sql(
            "grant usage on schema survey to team_assistant__api;",
            "add permissions on usage survey schema to team_assistant__api user");

        Execute.Sql(
            "alter default privileges in schema survey grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in survey for team_assistant__api user");

        Create
            .Table("questions")
            .InSchema("survey")
            
            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("questions__pk__id")
            
            .WithColumn("title")
            .AsString(50).NotNullable()
            
            .WithColumn("text")
            .AsString(2000).NotNullable();
        
        Create
            .Table("templates")
            .InSchema("survey")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("survey_templates__pk__id")

            .WithColumn("name")
            .AsString(255).NotNullable()

            .WithColumn("description")
            .AsString(2000).NotNullable()

            .WithColumn("question_ids")
            .AsCustom("jsonb").NotNullable();
        
        Create
            .Table("survey_rooms")
            .InSchema("survey")
            
            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("survey_rooms__pk__id")
            
            .WithColumn("template_id")
            .AsGuid().NotNullable()
            .ForeignKey("survey_rooms__fk__template_id", "survey", "templates", "id")
            
            .WithColumn("room_id")
            .AsGuid().NotNullable()
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("state")
            .AsInt32().NotNullable()
            
            .WithColumn("question_ids")
            .AsCustom("jsonb").NotNullable();
        
        Create
            .Table("survey_answers")
            .InSchema("survey")
            
            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("survey_answers__pk__id")
            
            .WithColumn("survey_id")
            .AsGuid().NotNullable()
            .ForeignKey("survey_answers__fk__survey_id", "survey", "survey_rooms", "id")
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("owner_id")
            .AsInt64().NotNullable()
            
            .WithColumn("answers")
            .AsCustom("jsonb").NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("survey_answers")
            .InSchema("survey");
        
        Delete
            .Table("survey_rooms")
            .InSchema("survey");
        
        Delete
            .Table("survey_templates")
            .InSchema("survey");
        
        Delete
            .Table("questions")
            .InSchema("survey");
        
        Delete
            .Schema("survey");
    }
}