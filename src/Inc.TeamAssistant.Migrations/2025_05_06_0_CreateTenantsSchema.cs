using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2025_05_06_0)]
public sealed class CreateTenantsSchema : Migration
{
    public override void Up()
    {
        Create
            .Schema("tenants");

        Execute.Sql(
            "grant usage on schema tenants to team_assistant__api;",
            "add permissions on usage tenants schema to team_assistant__api user");

        Execute.Sql(
            "alter default privileges in schema tenants grant select, update, insert, delete on tables to team_assistant__api;",
            "add select, update, insert privileges to all tables in tenants for team_assistant__api user");

        Create
            .Table("tenants")
            .InSchema("tenants")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("tenants__pk__id")

            .WithColumn("name")
            .AsString(50).NotNullable()

            .WithColumn("owner_id")
            .AsInt64().NotNullable();

        Create
            .Table("rooms")
            .InSchema("tenants")

            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("rooms__pk__id")

            .WithColumn("name")
            .AsString(50).NotNullable()

            .WithColumn("owner_id")
            .AsInt64().NotNullable()

            .WithColumn("tenant_id")
            .AsGuid().NotNullable()
            .ForeignKey(
                foreignKeyName: "rooms__tenants__fk__tenant_id__id",
                primaryTableSchema: "tenants",
                primaryTableName: "tenants",
                primaryColumnName: "id")
            
            .WithColumn("properties")
            .AsCustom("jsonb").NotNullable();
        
        Execute.Sql(
            "CREATE UNIQUE INDEX tenants__uidx__name ON tenants.tenants (lower(name));",
            "Create index by tenants name");
        
        Execute.Sql(
            "CREATE UNIQUE INDEX rooms__uidx__tenant_id__name ON tenants.rooms (tenant_id, lower(name));",
            "Create index by teams name");
        
        Alter
            .Column("name")
            .OnTable("teams")
            .InSchema("connector")
            .AsString(50).NotNullable();
    }

    public override void Down()
    {
        Alter
            .Column("name")
            .OnTable("teams")
            .InSchema("connector")
            .AsString(255).NotNullable();
        
        Delete
            .Table("rooms")
            .InSchema("tenants");
        
        Delete
            .Table("tenants")
            .InSchema("tenants");
        
        Delete
            .Schema("tenants");
    }
}