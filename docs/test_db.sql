-- connector
UPDATE connector.bots
SET
    name = 'inc_teamassistant_test_bot',
    token = ''
WHERE id = '8eace573-343b-4af7-b255-40c152d6832a';

DELETE FROM connector.activated_features
WHERE bot_id != '8eace573-343b-4af7-b255-40c152d6832a';

DELETE FROM connector.dashboard_settings
WHERE bot_id != '8eace573-343b-4af7-b255-40c152d6832a';

UPDATE connector.teams
SET
    bot_id = '8eace573-343b-4af7-b255-40c152d6832a',
    chat_id = -1001685108427;

DELETE FROM connector.bots
WHERE id != '8eace573-343b-4af7-b255-40c152d6832a';

-- appraiser
UPDATE appraiser.stories
SET
    bot_id = '8eace573-343b-4af7-b255-40c152d6832a',
    chat_id = -1001685108427;

-- maps
UPDATE maps.maps
SET
    bot_id = '8eace573-343b-4af7-b255-40c152d6832a',
    chat_id = 0;


-- random_coffee
DELETE FROM random_coffee.history
WHERE random_coffee_entry_id != '8daec45e-79cc-4ead-888f-2abc874176d9';

DELETE FROM random_coffee.entries
WHERE id != '8daec45e-79cc-4ead-888f-2abc874176d9';

UPDATE random_coffee.entries
SET
    bot_id = '8eace573-343b-4af7-b255-40c152d6832a',
    chat_id = -1001685108427
WHERE id = '8daec45e-79cc-4ead-888f-2abc874176d9';

-- review
UPDATE review.task_for_reviews
SET
    bot_id = '8eace573-343b-4af7-b255-40c152d6832a',
    chat_id = -1001685108427;

UPDATE review.task_for_reviews
SET
    state = 5,
    accept_date = now()
WHERE state in (1, 2, 3, 4);

UPDATE review.draft_task_for_reviews
SET chat_id = -1001685108427;