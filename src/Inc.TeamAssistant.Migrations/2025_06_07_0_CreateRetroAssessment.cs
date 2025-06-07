using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_06_07_0)]
public sealed class CreateRetroAssessment : Migration
{
    public override void Up()
    {
        Create
            .Table("retro_assessments")
            .InSchema("retro")

            .WithColumn("retro_session_id")
            .AsGuid().NotNullable()
            .PrimaryKey("retro_assessments__pk__r_s_id__p_id")
            .ForeignKey("retro_assessments__fk__retro_session_id", "retro", "retro_sessions", "id")

            .WithColumn("person_id")
            .AsInt64().NotNullable()
            .PrimaryKey("retro_assessments__pk__r_s_id__p_id")

            .WithColumn("value")
            .AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("retro_assessments")
            .InSchema("retro");
    }
}