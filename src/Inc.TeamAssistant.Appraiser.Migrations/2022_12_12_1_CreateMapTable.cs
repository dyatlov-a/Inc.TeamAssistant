using FluentMigrator;

namespace Inc.TeamAssistant.Appraiser.Migrations;

[Migration(2022_12_12_1)]
public sealed class CreateMapTable : Migration
{
    public override void Up()
    {
        Create
            .Table("maps")
            .InSchema("maps")

            .WithColumn("id")
            .AsGuid()
            .NotNullable()
            .PrimaryKey("maps__pk__id")

            .WithColumn("chat_id")
            .AsInt64()
            .NotNullable();

        Create
            .Table("locations")
            .InSchema("maps")

            .WithColumn("id")
            .AsGuid()
            .NotNullable()
            .PrimaryKey("locations__pk__id")

            .WithColumn("map_id")
            .AsGuid()
            .NotNullable()
            .ForeignKey(
                foreignKeyName: "locations__maps__fk__map_id__id",
                primaryTableSchema: "maps",
                primaryTableName: "maps",
                primaryColumnName: "id")

            .WithColumn("user_id")
            .AsInt64()
            .NotNullable()

            .WithColumn("display_name")
            .AsString()
            .NotNullable()

            .WithColumn("longitude")
            .AsDouble()
            .NotNullable()

            .WithColumn("latitude")
            .AsDouble()
            .NotNullable()

            .WithColumn("created")
            .AsDateTimeOffset()
            .NotNullable()

            .WithColumn("data")
            .AsCustom("jsonb")
            .NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("locations")
            .InSchema("maps");

        Delete
            .Table("maps")
            .InSchema("maps");
    }
}