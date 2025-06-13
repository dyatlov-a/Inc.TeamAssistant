using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_06_13_0)]
public sealed class CreateRetroTemplates : Migration
{
    public override void Up()
    {
        Create
            .Table("templates")
            .InSchema("retro")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("templates__pk__id")

            .WithColumn("name")
            .AsString(50).NotNullable();

        Create
            .Table("columns")
            .InSchema("retro")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("columns__pk__id")

            .WithColumn("template_id")
            .AsGuid().NotNullable()
            .ForeignKey("columns__fk__template_id", "retro", "templates", "id")
            
            .WithColumn("name")
            .AsString(50).NotNullable()
            
            .WithColumn("position")
            .AsInt32().NotNullable()
            
            .WithColumn("color")
            .AsString(7).NotNullable()
            
            .WithColumn("description")
            .AsString(255).Nullable();

        Execute.Sql(
            """
            INSERT INTO retro.templates (id, name)
            VALUES
                ('41c7a7b9-044f-46aa-b94e-e3bb06aed70c', 'TemplateStartStopContinue'),
                ('fc00fd5c-4db6-4760-b9e3-78814b60d392', 'TemplateToDiscussDiscussingDiscussed');
            """,
            "Set default templates");
        
        Execute.Sql(
            """
            INSERT INTO retro.columns (id, template_id, name, position, color, description)
            VALUES
                ('5561fbb9-6a39-442d-ae09-d26fe129248c', '41c7a7b9-044f-46aa-b94e-e3bb06aed70c', 'ColumnStartTitle', 1, '#FF8C00', 'ColumnStartDescription'),
                ('7855aa98-c74f-4104-aecd-339a3794aa8d', '41c7a7b9-044f-46aa-b94e-e3bb06aed70c', 'ColumnStopTitle', 2, '#B22222', 'ColumnStopDescription'),
                ('fc62de57-158d-490a-aaf9-3ec67f1f8583', '41c7a7b9-044f-46aa-b94e-e3bb06aed70c', 'ColumnContinueTitle', 3, '#3CB371', 'ColumnContinueDescription'),
                
                ('785859ec-6ea4-4d54-b69a-0323ce5844b0', 'fc00fd5c-4db6-4760-b9e3-78814b60d392', 'ColumnToDiscussTitle', 1, '#FF8C00', null),
                ('0f0549cc-8c7e-4694-b00e-6dc402e03825', 'fc00fd5c-4db6-4760-b9e3-78814b60d392', 'ColumnDiscussingTitle', 2, '#B22222', null),
                ('f94217cd-f405-451d-87b1-f58e50e56089', 'fc00fd5c-4db6-4760-b9e3-78814b60d392', 'ColumnDiscussedTitle', 3, '#3CB371', null);
            """,
            "");
    }

    public override void Down()
    {
        Delete
            .Table("columns")
            .InSchema("retro");
        
        Delete
            .Table("templates")
            .InSchema("retro");
    }
}