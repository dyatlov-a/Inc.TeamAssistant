using System.Data;
using FluentMigrator;

namespace Inc.TeamAssistant.Migrations;

[Migration(2024_05_02_0)]
public sealed class ChangeBotModel : Migration
{
    public override void Up()
    {
        Create
            .Table("features")
            .InSchema("connector")
            
            .WithColumn("id")
            .AsGuid().NotNullable()
            .PrimaryKey("features__pk__id")
            
            .WithColumn("name")
            .AsString(50).NotNullable();
            
        Create
            .Table("activated_features")
            .InSchema("connector")
            
            .WithColumn("feature_id")
            .AsGuid().NotNullable()
            .PrimaryKey("activated_features__pk__feature_id__bot_id")
            .ForeignKey(
                foreignKeyName: "activated_features__fk__feature_id__id",
                primaryColumnName: "id",
                primaryTableName: "features",
                primaryTableSchema: "connector")
            .OnDelete(Rule.Cascade)
            
            .WithColumn("bot_id")
            .AsGuid().NotNullable()
            .PrimaryKey("activated_features__pk__feature_id__bot_id")
            .ForeignKey(
                foreignKeyName: "activated_features__fk__bot_id__id",
                primaryColumnName: "id",
                primaryTableName: "bots",
                primaryTableSchema: "connector")
            .OnDelete(Rule.Cascade);

        Create
            .Table("command_packs")
            .InSchema("connector")

            .WithColumn("feature_id")
            .AsGuid().NotNullable()
            .PrimaryKey("command_packs__pk__feature_id__bot_command_id")
            .ForeignKey(
                foreignKeyName: "command_packs__fk__feature_id__id",
                primaryColumnName: "id",
                primaryTableName: "features",
                primaryTableSchema: "connector")
            .OnDelete(Rule.Cascade)
            
            .WithColumn("bot_command_id")
            .AsGuid().NotNullable()
            .PrimaryKey("command_packs__pk__feature_id__bot_command_id")
            .ForeignKey(
                foreignKeyName: "command_packs__fk__bot_command_id__id",
                primaryColumnName: "id",
                primaryTableName: "bot_commands",
                primaryTableSchema: "connector")
            .OnDelete(Rule.Cascade);

        Create
            .Column("owner_id")
            .OnTable("bots")
            .InSchema("connector")
            .AsInt64().NotNullable().SetExistingRowsTo(272062137);

        Create
            .Column("properties")
            .OnTable("bots")
            .InSchema("connector")
            .AsCustom("jsonb").NotNullable().SetExistingRowsTo("{}");
        
        Execute.Sql("DELETE FROM connector.bot_command_stages;", "Clean connector.bot_command_stages");
        
        Execute.Sql("DELETE FROM connector.bot_commands;", "Clean connector.bot_commands");
        
        Delete
            .ForeignKey("bot_commands__fk__bot_id__id")
            .OnTable("bot_commands")
            .InSchema("connector");
        
        Delete
            .Column("bot_id")
            .FromTable("bot_commands")
            .InSchema("connector");
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_commands(id, value, help_message_id)
            VALUES
                ('ce23b15d-ebe3-429f-a019-21671fe42bcd', '/new_team', 'Connector_NewTeamHelp'),
                ('54d07c42-0d8b-448c-a657-9ba508f30537', '/start', null),
                ('6d218fdd-058b-49fb-9422-08741c872a61', '/leave_team', 'Connector_LeaveFromTeamHelp'),
                ('4367a88f-d818-4239-93b9-826f70950cc4', '/cancel', 'Connector_CancelHelp'),
                ('ec16e7f2-3540-4252-a683-f1e9bbbfdac9', '/remove_team', 'Connector_RemoveTeamHelp'),
                ('f48db5a5-cbbe-45c6-9791-0bda8fd825fd', '/help', 'Connector_Help'),
                ('3aaaa192-3dde-4d41-a720-3ae65594aa11', '/add', 'Appraiser_AddHelp'),
                ('7203e75b-8675-4af0-a96b-84afb8aa4710', '/set', null),
                ('589978b5-9946-493a-9466-3402198d1262', '/accept', null),
                ('025740eb-b97e-43f1-9323-b7f35f8a6bd1', '/revote', null),
                ('3c1115b5-5bd0-4f30-b1d5-d7d23da59a2a', '/finish', null),
                ('57e1da3c-a65d-4172-b377-00d8543f55c3', '/need_review', 'Reviewer_MoveToReviewHelp'),
                ('4b8af862-51a9-42ab-9478-99ec92ceeb47', '/in_progress', null),
                ('b009ca75-a7b5-4159-8664-81caa30f5819', '/approve', null),
                ('f3a22d65-2f86-4cc1-8945-7c6a36addc6f', '/decline', null),
                ('1c3bfc0c-846a-470b-8433-8ca599441164', '/next_round', null),
                ('f6b6aa8c-1ddf-4446-b91a-6e223108f366', '/reassign', null),
                ('4378f324-4d70-4d53-b934-f5fdad6bb42b', '/location', 'CheckIn_AddLocationHelp'),
                ('687426da-75eb-4cea-bdb1-6afd6c7e5dc0', '/invite', 'RandomCoffee_InviteHelp'),
                ('a85b8851-6939-47f5-8e35-9fe9ecf680f7', '/poll_answer', null)
            """,
            "Setup bot_commands");
        
        Execute.Sql(
            """
            INSERT INTO connector.bot_command_stages(id, bot_command_id, value, dialog_message_id, position)
            VALUES
                ('7f6aecc5-b33a-4cd9-b52f-8c7012035939', 'ce23b15d-ebe3-429f-a019-21671fe42bcd', 1, 'Connector_EnterTeamName', 1),
                ('aadd6828-a785-42ac-bb81-7daee97aa6fb', '6d218fdd-058b-49fb-9422-08741c872a61', 1, 'Connector_SelectTeam', 1),
                ('14f95673-dbe0-4c06-9395-3e72e83ea52a', '3aaaa192-3dde-4d41-a720-3ae65594aa11', 1, 'Connector_SelectTeam', 1),
                ('7725d913-19b9-48c8-9d0c-bafb89a972f3', '3aaaa192-3dde-4d41-a720-3ae65594aa11', 2, 'Appraiser_EnterStoryName', 2),
                ('35569ffd-dc1a-4b10-be72-66725d6569f5', '57e1da3c-a65d-4172-b377-00d8543f55c3', 1, 'Connector_SelectTeam', 1),
                ('a225f93e-36ba-4bf7-9b7c-9cb82fa84c09', '57e1da3c-a65d-4172-b377-00d8543f55c3', 2, 'Reviewer_EnterRequestForReview', 2),
                ('4df5448c-8ee1-4002-bbfe-a75b71c38d68', '4378f324-4d70-4d53-b934-f5fdad6bb42b', 1, 'CheckIn_AddLocation', 1),
                ('c014bdfb-1c54-44eb-87ef-a6561e5204b4', 'ec16e7f2-3540-4252-a683-f1e9bbbfdac9', 1, 'Connector_SelectTeam', 1)
            """,
            "Setup bot_command_stages");
        
        Execute.Sql(
            """
                INSERT INTO connector.features(id, name)
                VALUES
                    ('5a7334e6-8076-4fc1-89e9-5139b8135947', 'Appraiser'),
                    ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'Reviewer'),
                    ('a8623f4a-5ac6-40e5-8d38-e3d76f641dc7', 'CheckIn'),
                    ('39195e70-b83a-42b3-88e5-dbbf6789a3c8', 'RandomCoffee')
            """,
            "Setup features");
        
        Execute.Sql(
            """
            INSERT INTO connector.command_packs(feature_id, bot_command_id)
            VALUES
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', 'ce23b15d-ebe3-429f-a019-21671fe42bcd'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '54d07c42-0d8b-448c-a657-9ba508f30537'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '6d218fdd-058b-49fb-9422-08741c872a61'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '4367a88f-d818-4239-93b9-826f70950cc4'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', 'ec16e7f2-3540-4252-a683-f1e9bbbfdac9'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', 'f48db5a5-cbbe-45c6-9791-0bda8fd825fd'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '3aaaa192-3dde-4d41-a720-3ae65594aa11'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '7203e75b-8675-4af0-a96b-84afb8aa4710'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '589978b5-9946-493a-9466-3402198d1262'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '025740eb-b97e-43f1-9323-b7f35f8a6bd1'),
                ('5a7334e6-8076-4fc1-89e9-5139b8135947', '3c1115b5-5bd0-4f30-b1d5-d7d23da59a2a'),
                
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'ce23b15d-ebe3-429f-a019-21671fe42bcd'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '54d07c42-0d8b-448c-a657-9ba508f30537'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '6d218fdd-058b-49fb-9422-08741c872a61'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '4367a88f-d818-4239-93b9-826f70950cc4'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'ec16e7f2-3540-4252-a683-f1e9bbbfdac9'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'f48db5a5-cbbe-45c6-9791-0bda8fd825fd'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '57e1da3c-a65d-4172-b377-00d8543f55c3'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '4b8af862-51a9-42ab-9478-99ec92ceeb47'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'b009ca75-a7b5-4159-8664-81caa30f5819'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'f3a22d65-2f86-4cc1-8945-7c6a36addc6f'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', '1c3bfc0c-846a-470b-8433-8ca599441164'),
                ('501df55a-42db-4db6-a057-e5a4d3ed3625', 'f6b6aa8c-1ddf-4446-b91a-6e223108f366'),
                
                ('a8623f4a-5ac6-40e5-8d38-e3d76f641dc7', '4378f324-4d70-4d53-b934-f5fdad6bb42b'),
                ('a8623f4a-5ac6-40e5-8d38-e3d76f641dc7', '4367a88f-d818-4239-93b9-826f70950cc4'),
                ('a8623f4a-5ac6-40e5-8d38-e3d76f641dc7', 'f48db5a5-cbbe-45c6-9791-0bda8fd825fd'),
                
                ('39195e70-b83a-42b3-88e5-dbbf6789a3c8', '687426da-75eb-4cea-bdb1-6afd6c7e5dc0'),
                ('39195e70-b83a-42b3-88e5-dbbf6789a3c8', 'a85b8851-6939-47f5-8e35-9fe9ecf680f7'),
                ('39195e70-b83a-42b3-88e5-dbbf6789a3c8', 'f48db5a5-cbbe-45c6-9791-0bda8fd825fd')
            """,
            "Setup command_packs");
    }

    public override void Down()
    {
    }
}