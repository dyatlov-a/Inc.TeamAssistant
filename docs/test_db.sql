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
SET bot_id = '8eace573-343b-4af7-b255-40c152d6832a';

DELETE FROM connector.bots
WHERE id != '8eace573-343b-4af7-b255-40c152d6832a';

-- appraiser
UPDATE appraiser.stories
SET bot_id = '8eace573-343b-4af7-b255-40c152d6832a';

-- maps
UPDATE maps.maps
SET bot_id = '8eace573-343b-4af7-b255-40c152d6832a';

-- random_coffee
UPDATE random_coffee.entries
SET bot_id = '8eace573-343b-4af7-b255-40c152d6832a';

-- review
UPDATE review.task_for_reviews
SET bot_id = '8eace573-343b-4af7-b255-40c152d6832a';