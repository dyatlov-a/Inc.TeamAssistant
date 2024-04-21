INSERT INTO connector.bots(id, name, token)
VALUES
    ('2c9e14e0-63be-4381-aca0-a0e848cd34bc', 'inc_teamassistant_appraiser_bot', 'hidden'),
    ('e5b2d82b-3912-4d94-acb8-c6e603622a95', 'inc_teamassistant_reviewer_bot', 'hidden'),
    ('29160ded-0e79-4bce-9856-52eb39657e8d', 'inc_teamassistant_checkin_bot', 'hidden'),
    ('97e6239e-4f51-4165-9a69-1398cc0b6b93', 'inc_teamassistant_rnd_coffee_bot', 'hidden')
ON CONFLICT (id) DO UPDATE SET
    name = excluded.name,
    token = excluded.token;

INSERT INTO connector.bot_commands(id, bot_id, value, help_message_id)
VALUES
    ('ce23b15d-ebe3-429f-a019-21671fe42bcd', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/new_team', 'Connector_NewTeamHelp'),
    ('54d07c42-0d8b-448c-a657-9ba508f30537', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/start', null),
    ('6d218fdd-058b-49fb-9422-08741c872a61', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/leave_team', 'Connector_LeaveFromTeamHelp'),
    ('4367a88f-d818-4239-93b9-826f70950cc4', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/cancel', 'Connector_CancelHelp'),
    ('d2572173-b1cc-486b-9dee-207480601cfb', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/remove_team', 'Connector_RemoveTeamHelp'),
    ('f48db5a5-cbbe-45c6-9791-0bda8fd825fd', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/help', 'Connector_Help'),

    ('3aaaa192-3dde-4d41-a720-3ae65594aa11', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/add', 'Appraiser_AddHelp'),
    ('7203e75b-8675-4af0-a96b-84afb8aa4710', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/set', null),
    ('3c1115b5-5bd0-4f30-b1d5-d7d23da59a2a', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/finish', null),
    ('589978b5-9946-493a-9466-3402198d1262', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/accept', null),
    ('025740eb-b97e-43f1-9323-b7f35f8a6bd1', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/revote', null),
    ('d753d196-0c01-46ac-83d1-65fdd5d40318', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/move_to_sp', 'Appraiser_MoveToSpHelp'),
    ('a49e0652-9237-455e-afac-d6119fe10fde', '2c9e14e0-63be-4381-aca0-a0e848cd34bc', '/move_to_tshirts', 'Appraiser_MoveToTShirtsHelp'),

    ('2cf2974a-1fb8-47c7-869e-fda802924ad4', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/new_team', 'Connector_NewTeamHelp'),
    ('c670b665-1dde-438a-97dc-7e2873f2cbbd', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/start', null),
    ('3a8e95ba-0b62-4bd1-b59a-261bdbc14532', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/leave_team', 'Connector_LeaveFromTeamHelp'),
    ('f2d2cb41-f07f-4b52-b5d1-8e3981bdf159', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/cancel', 'Connector_CancelHelp'),
    ('ec16e7f2-3540-4252-a683-f1e9bbbfdac9', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/remove_team', 'Connector_RemoveTeamHelp'),
    ('7c7a447e-af64-4a0e-b9d2-513ea76a447d', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/help', 'Connector_Help'),

    ('57e1da3c-a65d-4172-b377-00d8543f55c3', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/need_review', 'Reviewer_MoveToReviewHelp'),
    ('4b8af862-51a9-42ab-9478-99ec92ceeb47', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/in_progress', null),
    ('b009ca75-a7b5-4159-8664-81caa30f5819', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/approve', null),
    ('f3a22d65-2f86-4cc1-8945-7c6a36addc6f', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/decline', null),
    ('1c3bfc0c-846a-470b-8433-8ca599441164', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/next_round', null),
    ('e9b79c43-1bd5-4bf8-91e7-6677b5d72e71', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/change_to_round_robin', 'Reviewer_ChangeToRoundRobinHelp'),
    ('10b888d0-e086-4a51-85d2-6379735682c7', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/change_to_random', 'Reviewer_ChangeToRandomHelp'),
    ('f6b6aa8c-1ddf-4446-b91a-6e223108f366', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/reassign', null),

    ('4f5843de-4a65-4934-afd7-efa6dfbf22fa', '29160ded-0e79-4bce-9856-52eb39657e8d', '/location', 'CheckIn_AddLocationHelp'),
    ('291e4bd8-76b5-4d37-ab2d-1856ac182925', '29160ded-0e79-4bce-9856-52eb39657e8d', '/help', 'Connector_Help'),
    
    ('687426da-75eb-4cea-bdb1-6afd6c7e5dc0', '97e6239e-4f51-4165-9a69-1398cc0b6b93', '/invite', 'RandomCoffee_InviteHelp'),
    ('a85b8851-6939-47f5-8e35-9fe9ecf680f7', '97e6239e-4f51-4165-9a69-1398cc0b6b93', '/poll_answer', null),
    ('36a949b7-c7f8-4757-8ce8-698b3d2ece26', '97e6239e-4f51-4165-9a69-1398cc0b6b93', '/help', 'Connector_Help')
ON CONFLICT (id) DO UPDATE SET
    bot_id = excluded.bot_id,
    value = excluded.value,
    help_message_id = excluded.help_message_id;

INSERT INTO connector.bot_command_stages(id, bot_command_id, value, dialog_message_id, position)
VALUES
    ('7f6aecc5-b33a-4cd9-b52f-8c7012035939', 'ce23b15d-ebe3-429f-a019-21671fe42bcd', 1, 'Connector_EnterTeamName', 1),
    ('aadd6828-a785-42ac-bb81-7daee97aa6fb', '6d218fdd-058b-49fb-9422-08741c872a61', 1, 'Connector_SelectTeam', 1),
    ('14f95673-dbe0-4c06-9395-3e72e83ea52a', '3aaaa192-3dde-4d41-a720-3ae65594aa11', 1, 'Connector_SelectTeam', 1),
    ('7725d913-19b9-48c8-9d0c-bafb89a972f3', '3aaaa192-3dde-4d41-a720-3ae65594aa11', 2, 'Appraiser_EnterStoryName', 2),
    ('35569ffd-dc1a-4b10-be72-66725d6569f5', '57e1da3c-a65d-4172-b377-00d8543f55c3', 1, 'Connector_SelectTeam', 1),
    ('a225f93e-36ba-4bf7-9b7c-9cb82fa84c09', '57e1da3c-a65d-4172-b377-00d8543f55c3', 2, 'Reviewer_EnterRequestForReview', 2),
    ('29a7185b-ceff-4678-9030-17907665bc01', 'd753d196-0c01-46ac-83d1-65fdd5d40318', 1, 'Connector_SelectTeam', 1),
    ('eddcadc0-6805-4366-8c3b-0c5888d40539', 'a49e0652-9237-455e-afac-d6119fe10fde', 1, 'Connector_SelectTeam', 1),
    ('1c6271aa-8051-4cc4-b3ff-d7b7fabf1020', 'e9b79c43-1bd5-4bf8-91e7-6677b5d72e71', 1, 'Connector_SelectTeam', 1),
    ('697078dc-94c2-43a3-abd7-762641c1d094', '10b888d0-e086-4a51-85d2-6379735682c7', 1, 'Connector_SelectTeam', 1),
    ('c014bdfb-1c54-44eb-87ef-a6561e5204b4', 'd2572173-b1cc-486b-9dee-207480601cfb', 1, 'Connector_SelectTeam', 1),
    ('16438738-cc3d-48b0-a81f-075074055f28', 'ec16e7f2-3540-4252-a683-f1e9bbbfdac9', 1, 'Connector_SelectTeam', 1),
    
    ('4df5448c-8ee1-4002-bbfe-a75b71c38d68', '4f5843de-4a65-4934-afd7-efa6dfbf22fa', 1, 'CheckIn_AddLocation', 1),

    ('62dc2e65-fba8-4ce3-b0a5-cef36f988708', '2cf2974a-1fb8-47c7-869e-fda802924ad4', 1, 'Connector_EnterTeamName', 1),
    ('4258f5bd-8136-4e03-b0e8-e601286523f8', '3a8e95ba-0b62-4bd1-b59a-261bdbc14532', 1, 'Connector_SelectTeam', 1)
ON CONFLICT (id) DO UPDATE SET
    bot_command_id = excluded.bot_command_id,
    value = excluded.value;