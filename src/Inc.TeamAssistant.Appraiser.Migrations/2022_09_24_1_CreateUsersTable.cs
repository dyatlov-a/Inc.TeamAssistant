using FluentMigrator;

namespace Inc.TeamAssistant.Appraiser.Migrations;

[Migration(2022_09_24_1)]
public sealed class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create
            .Table("users")
            .InSchema("users")

            .WithColumn("id")
            .AsInt64()
            .NotNullable()
            .PrimaryKey("users__pk__id")

            .WithColumn("name")
            .AsString()
            .NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("users")
            .InSchema("users");
    }
}