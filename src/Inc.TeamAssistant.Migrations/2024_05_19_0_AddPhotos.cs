using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_05_19_0)]
public sealed class AddPhotos : Migration
{
    public override void Up()
    {
        Create
            .Table("photos")
            .InSchema("connector")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("photos__pk__id")
            
            .WithColumn("person_id")
            .AsInt64().NotNullable()
            
            .WithColumn("date")
            .AsDateTimeOffset().NotNullable()
            
            .WithColumn("data")
            .AsBinary().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("photos")
            .InSchema("connector");
    }
}