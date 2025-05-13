using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2023_11_07_0)]
public sealed class DeleteUsersSchema : Migration
{
    public override void Up()
    {
        Delete
            .Table("users")
            .InSchema("users");
        
        Delete
            .Schema("users");
    }

    public override void Down()
    {
    }
}