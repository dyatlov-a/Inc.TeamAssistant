INSERT INTO connector.bots(id, name, token)
VALUES
    ('e5b2d82b-3912-4d94-acb8-c6e603622a95', 'inc_teamassistant_test_bot', '5950633493:AAFU5Lg_lWCptt8jh05r8SnP5jcLaf8CL84')
ON CONFLICT (id) DO UPDATE SET
    name = excluded.name,
    token = excluded.token;

INSERT INTO connector.bot_commands(id, bot_id, value, help_message_id)
VALUES
    ('ce23b15d-ebe3-429f-a019-21671fe42bcd', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/new_team', 'Connector_NewTeamHelp'),
    ('54d07c42-0d8b-448c-a657-9ba508f30537', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/start', null),
    ('6d218fdd-058b-49fb-9422-08741c872a61', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/leave_team', 'Connector_LeaveFromTeamHelp'),
    ('4367a88f-d818-4239-93b9-826f70950cc4', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/cancel', 'Connector_CancelHelp'),
    ('f48db5a5-cbbe-45c6-9791-0bda8fd825fd', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/help', null),

    ('3aaaa192-3dde-4d41-a720-3ae65594aa11', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/add', 'Appraiser_AddHelp'),
    ('7203e75b-8675-4af0-a96b-84afb8aa4710', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/set', null),
    ('589978b5-9946-493a-9466-3402198d1262', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/accept', null),
    ('025740eb-b97e-43f1-9323-b7f35f8a6bd1', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/revote', null),
    ('d753d196-0c01-46ac-83d1-65fdd5d40318', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/move_to_sp', 'Appraiser_MoveToSpHelp'),
    ('a49e0652-9237-455e-afac-d6119fe10fde', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/move_to_tshirts', 'Appraiser_MoveToTShirtsHelp'),
    ('3c1115b5-5bd0-4f30-b1d5-d7d23da59a2a', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/finish', null),

    ('57e1da3c-a65d-4172-b377-00d8543f55c3', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/need_review', 'Reviewer_MoveToReviewHelp'),
    ('4b8af862-51a9-42ab-9478-99ec92ceeb47', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/in_progress', null),
    ('b009ca75-a7b5-4159-8664-81caa30f5819', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/approve', null),
    ('f3a22d65-2f86-4cc1-8945-7c6a36addc6f', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/decline', null),
    ('1c3bfc0c-846a-470b-8433-8ca599441164', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/next_round', null),
    ('e9b79c43-1bd5-4bf8-91e7-6677b5d72e71', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/change_to_round_robin', 'Reviewer_ChangeToRoundRobinHelp'),
    ('10b888d0-e086-4a51-85d2-6379735682c7', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/change_to_random', 'Reviewer_ChangeToRandomHelp'),
    ('f6b6aa8c-1ddf-4446-b91a-6e223108f366', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/reassign', null),

    ('4378f324-4d70-4d53-b934-f5fdad6bb42b', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/location', 'CheckIn_AddLocationHelp'),

    ('687426da-75eb-4cea-bdb1-6afd6c7e5dc0', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/invite', 'RandomCoffee_InviteHelp'),
    ('a85b8851-6939-47f5-8e35-9fe9ecf680f7', 'e5b2d82b-3912-4d94-acb8-c6e603622a95', '/poll_answer', null)
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
    ('4df5448c-8ee1-4002-bbfe-a75b71c38d68', '4378f324-4d70-4d53-b934-f5fdad6bb42b', 1, 'CheckIn_AddLocation', 1)
ON CONFLICT (id) DO UPDATE SET
    bot_command_id = excluded.bot_command_id,
    value = excluded.value;