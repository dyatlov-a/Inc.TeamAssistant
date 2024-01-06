using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_01_03_1)]
public class CreateConnectorTables : Migration
{
    public override void Up()
    {
        Create
            .Table("bots")
            .InSchema("connector")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("bots__pk__id")
            
            .WithColumn("name")
            .AsString(50).NotNullable()
            
            .WithColumn("token")
            .AsString(255).NotNullable();
        
        Create
            .Table("persons")
            .InSchema("connector")

            .WithColumn("id")
            .AsInt64().NotNullable()
            .PrimaryKey("persons__pk__id")
            
            .WithColumn("name")
            .AsString().NotNullable()
            
            .WithColumn("language_id")
            .AsString().NotNullable()
            
            .WithColumn("username")
            .AsString().Nullable();

        Create
            .Table("teams")
            .InSchema("connector")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("teams__pk__id")

            .WithColumn("bot_id")
            .AsGuid().NotNullable()
            .ForeignKey(
                foreignKeyName: "teams__fk__bot_id__id",
                primaryColumnName: "id",
                primaryTableName: "bots",
                primaryTableSchema: "connector")

            .WithColumn("chat_id")
            .AsInt64().NotNullable()

            .WithColumn("name")
            .AsString(255).NotNullable();

        Create
            .Table("teammates")
            .InSchema("connector")
            
            .WithColumn("team_id")
            .AsGuid().NotNullable()
            .PrimaryKey("teammates__pk__team_id__person_id")
            .ForeignKey(
                foreignKeyName: "teammates__fk__team_id__id",
                primaryColumnName: "id",
                primaryTableName: "teams",
                primaryTableSchema: "connector")
            
            .WithColumn("person_id")
            .AsInt64().NotNullable()
            .PrimaryKey("teammates__pk__team_id__person_id")
            .ForeignKey(
                foreignKeyName: "teammates__fk__person_id__id",
                primaryColumnName: "id",
                primaryTableName: "persons",
                primaryTableSchema: "connector");

        Create
            .Table("bot_commands")
            .InSchema("connector")
            
            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("bot_commands__pk__id")
            
            .WithColumn("bot_id")
            .AsGuid().NotNullable()
            .ForeignKey(
                foreignKeyName: "bot_commands__fk__bot_id__id",
                primaryColumnName: "id",
                primaryTableName: "bots",
                primaryTableSchema: "connector")
            
            .WithColumn("value")
            .AsString(50).NotNullable()
            
            .WithColumn("help_message_id")
            .AsString(50).Nullable();

        Create
            .Table("bot_command_stages")
            .InSchema("connector")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("bot_command_stages__pk__id")
            
            .WithColumn("bot_command_id")
            .AsGuid().NotNullable()
            .ForeignKey(
                foreignKeyName: "bot_command_stages__fk__bot_command_id__id",
                primaryColumnName: "id",
                primaryTableName: "bot_commands",
                primaryTableSchema: "connector")
            
            .WithColumn("value")
            .AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("bot_command_stages")
            .InSchema("connector");
        
        Delete
            .Table("bot_commands")
            .InSchema("connector");
        
        Delete
            .Table("teammates")
            .InSchema("connector");
        
        Delete
            .Table("teams")
            .InSchema("connector");
        
        Delete
            .Table("persons")
            .InSchema("connector");
        
        Delete
            .Table("bots")
            .InSchema("connector");
    }
}