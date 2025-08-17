using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_08_15_0)]
public sealed class ChangeSurvey : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            UPDATE survey.templates
            SET question_ids = '["360c75d3-b62d-4475-8e08-f3ee1e9b97d3", "e8b24f0d-ebc3-4c57-b31d-524ef5f92cde", "7e65d47a-7031-4ac3-b5e9-bdc24a69e681", "7a6f830b-c75b-4d90-92aa-cbeeb061487e", "07ef9c9d-96f2-47dc-a236-d33ee3ecfa27", "8b1b6db6-e69c-450d-bb17-4c9ad4dfa77f", "b780db1e-76f2-4ae1-a5b4-40cec84da2c5", "94ec7e6a-924d-4d1e-933f-0ddc0072bf41"]'::JSONB
            WHERE id = '24038309-ba36-4838-adf6-315dfa3cf94b';
            """,
            "Update SurveyTemplateRemoteTeamHappiness");
        
        Execute.Sql(
            """
            INSERT INTO survey.questions (id, title, text)
            VALUES
                ('360c75d3-b62d-4475-8e08-f3ee1e9b97d3', 'TeamEngagementTitle', 'TeamEngagementText'),
                ('e8b24f0d-ebc3-4c57-b31d-524ef5f92cde', 'RoleClarityTitle', 'RoleClarityText'),
                ('7e65d47a-7031-4ac3-b5e9-bdc24a69e681', 'AutonomyTitle', 'AutonomyText'),
                ('7a6f830b-c75b-4d90-92aa-cbeeb061487e', 'RemoteSupportTitle', 'RemoteSupportText'),
                ('07ef9c9d-96f2-47dc-a236-d33ee3ecfa27', 'IsolationTitle', 'IsolationText'),
                ('8b1b6db6-e69c-450d-bb17-4c9ad4dfa77f', 'WellBeingTitle', 'WellBeingText'),
                ('b780db1e-76f2-4ae1-a5b4-40cec84da2c5', 'ConnectednessTitle', 'ConnectednessText'),
                ('94ec7e6a-924d-4d1e-933f-0ddc0072bf41', 'DistractionsTitle', 'DistractionsText')
            """,
            "Add questions for SurveyTemplateRemoteTeamHappiness");
        
        Delete
            .Table("survey_answers")
            .InSchema("survey");
        
        Create
            .Table("survey_answers")
            .InSchema("survey")
            
            .WithColumn("survey_id")
            .AsGuid().NotNullable()
            .PrimaryKey("survey_answers__pk__survey_id__question_id__responder_id")
            .ForeignKey("survey_answers__fk__survey_id", "survey", "surveys", "id")
            
            .WithColumn("question_id")
            .AsGuid().NotNullable()
            .PrimaryKey("survey_answers__pk__survey_id__question_id__responder_id")
            
            .WithColumn("responder_id")
            .AsInt64().NotNullable()
            .PrimaryKey("survey_answers__pk__survey_id__question_id__responder_id")
            
            .WithColumn("responded")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("value")
            .AsInt32().NotNullable()
            
            .WithColumn("comment")
            .AsString(250).Nullable();
        
        Delete
            .Column("question_ids")
            .FromTable("surveys")
            .InSchema("survey");
    }

    public override void Down()
    {
        Create
            .Column("question_ids")
            .OnTable("surveys")
            .InSchema("survey")
            .AsCustom("jsonb")
            .NotNullable()
            .SetExistingRowsTo("[]");
        
        Delete
            .Table("survey_answers")
            .InSchema("survey");
        
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
            DELETE survey.questions WHERE id IN ("360c75d3-b62d-4475-8e08-f3ee1e9b97d3", "e8b24f0d-ebc3-4c57-b31d-524ef5f92cde", "7e65d47a-7031-4ac3-b5e9-bdc24a69e681", "7a6f830b-c75b-4d90-92aa-cbeeb061487e", "07ef9c9d-96f2-47dc-a236-d33ee3ecfa27", "8b1b6db6-e69c-450d-bb17-4c9ad4dfa77f", "b780db1e-76f2-4ae1-a5b4-40cec84da2c5", "94ec7e6a-924d-4d1e-933f-0ddc0072bf41");
            """,
            "Remove questions from SurveyTemplateRemoteTeamHappiness");
    }
}