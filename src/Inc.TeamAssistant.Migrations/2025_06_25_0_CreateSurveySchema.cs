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
            .AsString(50).NotNullable()

            .WithColumn("question_ids")
            .AsCustom("jsonb").NotNullable();
        
        Create
            .Table("surveys")
            .InSchema("survey")
            
            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("surveys__pk__id")
            
            .WithColumn("template_id")
            .AsGuid().NotNullable()
            .ForeignKey("surveys__fk__template_id", "survey", "templates", "id")
            
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
            .ForeignKey("survey_answers__fk__survey_id", "survey", "surveys", "id")
            
            .WithColumn("created")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("owner_id")
            .AsInt64().NotNullable()
            
            .WithColumn("answers")
            .AsCustom("jsonb").NotNullable();
        
        Execute.Sql(
            """
            INSERT INTO survey.templates (id, name, question_ids)
            VALUES
                ('6c9b2eef-b7ce-4e13-b866-1a0cd743c6b3', 'SurveyTemplateSpotifyHealthCheck', '["a72fffac-fc78-4d31-b870-682207b065b4", "7655a7a6-377a-4c48-8def-ab9a0694c708", "a1183b99-b102-477d-93ba-7ef99121670b", "10eeb23b-669b-414a-8db1-1efdd8df4e19", "e9417f25-1e31-45ce-a2d6-a62eacc971fd", "e0087ef7-276d-4a44-b69c-f9b820ae008c", "f1c21e84-ff4b-46f9-9d26-2305acbde82a", "8dfcc815-0ae0-46d7-a20e-d9ded64e4b1b", "2472847c-21f3-4565-853f-caa406d89a25", "01182478-ad44-4e27-90d1-3ffa604766c0", "f9ded0f4-4379-4993-a08b-527930100ef6"]'::JSONB),
                ('24038309-ba36-4838-adf6-315dfa3cf94b', 'SurveyTemplateRemoteTeamHappiness', '[]'::JSONB);
            """,
            "Set default templates");
        
        Execute.Sql(
            """
            INSERT INTO survey.questions (id, title, text)
            VALUES
                ('a72fffac-fc78-4d31-b870-682207b065b4', 'DeliveringValueTitle', 'DeliveringValueText'),
                ('7655a7a6-377a-4c48-8def-ab9a0694c708', 'EasyToReleaseTitle', 'EasyToReleaseText'),
                ('a1183b99-b102-477d-93ba-7ef99121670b', 'FunTitle', 'FunText'),
                ('f9ded0f4-4379-4993-a08b-527930100ef6', 'HealthOfCodebaseTitle', 'HealthOfCodebaseText'),
                ('10eeb23b-669b-414a-8db1-1efdd8df4e19', 'LearningTitle', 'LearningText'),
                ('e9417f25-1e31-45ce-a2d6-a62eacc971fd', 'MissionTitle', 'MissionText'),
                ('e0087ef7-276d-4a44-b69c-f9b820ae008c', 'PlayerOrPawnTitle', 'PlayerOrPawnText'),
                ('f1c21e84-ff4b-46f9-9d26-2305acbde82a', 'SpeedTitle', 'SpeedText'),
                ('8dfcc815-0ae0-46d7-a20e-d9ded64e4b1b', 'SuitableProcessesTitle', 'SuitableProcessesText'),
                ('2472847c-21f3-4565-853f-caa406d89a25', 'SupportTitle', 'SupportText'),
                ('01182478-ad44-4e27-90d1-3ffa604766c0', 'TeamworkTitle', 'TeamworkText');
            """,
            "Add questions");
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