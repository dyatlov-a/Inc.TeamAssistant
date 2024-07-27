using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_07_27_0)]
public sealed class DeletePhotos : Migration
{
    public override void Up()
    {
        Delete
            .Table("photos")
            .InSchema("connector");
    }

    public override void Down()
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
}