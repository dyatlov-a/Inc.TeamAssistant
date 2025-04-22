using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class TaskForReviewReader : ITaskForReviewReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public TaskForReviewReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);

        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.bot_id AS botid,
                t.team_id AS teamid,
                t.strategy AS strategy,
                t.owner_id AS ownerid,
                t.reviewer_id AS reviewerid,
                t.description AS description,
                t.state AS state,
                t.created AS created,
                t.next_notification AS nextnotification,
                t.accept_date AS acceptdate,
                t.message_id AS messageid,
                t.chat_id AS chatid,
                t.original_reviewer_id AS originalreviewerid,
                t.original_reviewer_message_id AS originalreviewermessageid,
                t.first_reviewer_id AS firstreviewerid,
                t.second_reviewer_id AS secondreviewerid,
                t.review_intervals AS reviewintervals
            FROM review.task_for_reviews AS t
            WHERE t.state = ANY(@states) AND t.next_notification < @now
            ORDER BY t.next_notification;           
            """,
            new
            {
                now = now.UtcDateTime,
                states = targetStates
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<TaskForReview>(command);

        return results.ToArray();
    }

    public async Task<IReadOnlyCollection<TaskForReview>> GetTasksByPerson(
        Guid teamId,
        long personId,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);

        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                bot_id AS botid,
                t.team_id AS teamid,
                t.strategy AS strategy,
                t.owner_id AS ownerid,
                t.owner_message_id AS ownermessageid,
                t.reviewer_id AS reviewerid,
                t.reviewer_message_id AS reviewermessageid,
                t.description AS description,
                t.state AS state,
                t.created AS created,
                t.next_notification AS nextnotification,
                t.accept_date AS acceptdate,
                t.message_id AS messageid,
                t.chat_id AS chatid,
                t.original_reviewer_id AS originalreviewerid,
                t.original_reviewer_message_id AS originalreviewermessageid,
                t.first_reviewer_id AS firstreviewerid,
                t.second_reviewer_id AS secondreviewerid,
                t.review_intervals AS reviewintervals
            FROM review.task_for_reviews AS t
            WHERE t.team_id = @team_id AND t.reviewer_id = @person_id AND t.state = ANY(@states);           
            """,
            new
            {
                team_id = teamId,
                person_id = personId,
                states = targetStates
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<TaskForReview>(command);

        return results.ToArray();
    }

    public async Task<IReadOnlyCollection<TaskForReview>> GetTasksFrom(
        Guid? teamId,
        DateTimeOffset date,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                bot_id AS botid,
                t.team_id AS teamid,
                t.strategy AS strategy,
                t.owner_id AS ownerid,
                t.reviewer_id AS reviewerid,
                t.description AS description,
                t.state AS state,
                t.created AS created,
                t.next_notification AS nextnotification,
                t.accept_date AS acceptdate,
                t.message_id AS messageid,
                t.chat_id AS chatid,
                t.original_reviewer_id AS originalreviewerid,
                t.original_reviewer_message_id AS originalreviewermessageid,
                t.first_reviewer_id AS firstreviewerid,
                t.second_reviewer_id AS secondreviewerid,
                t.review_intervals AS reviewintervals
            FROM review.task_for_reviews AS t
            WHERE (@team_id IS NULL OR t.team_id = @team_id) AND t.state = @target_status AND t.created > @date
            ORDER BY t.created;         
            """,
            new
            {
                team_id = teamId,
                date = date.UtcDateTime,
                target_status = (int)TaskForReviewState.Accept
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<TaskForReview>(command);

        return results.ToArray();
    }

    public async Task<IReadOnlyDictionary<long, int>> GetHistory(
        Guid teamId,
        DateTimeOffset date,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.first_reviewer_id AS reviewerid,
                COUNT(*) AS count
            FROM review.task_for_reviews AS t
            WHERE t.team_id = @team_id AND t.first_reviewer_id IS NOT NULL AND t.created >= @date
            GROUP BY t.first_reviewer_id;
            """,
            new
            {
                team_id = teamId,
                date = date.UtcDateTime
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var history = await connection.QueryAsync<(long ReviewerId, int Count)>(command);

        return history.ToDictionary(h => h.ReviewerId, h => h.Count);
    }

    public async Task<IReadOnlyCollection<TaskForReviewHistory>> GetLastTasks(
        Guid teamId,
        DateTimeOffset from,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.created AS created,
                t.state AS state,
                t.strategy AS strategy,
                t.bot_id AS botid,
                t.description AS description,
                t.review_intervals AS reviewintervals,
                r.id AS reviewerid,
                r.name AS reviewername,
                r.username AS reviewerusername,
                o.id AS ownerid,
                o.name AS ownername,
                o.username AS ownerusername,
                t.original_reviewer_id AS originalreviewerid
            FROM review.task_for_reviews AS t
            JOIN connector.persons AS r ON r.id = t.reviewer_id
            JOIN connector.persons AS o ON o.id = t.owner_id
            WHERE t.team_id = @team_id AND t.created >= @from
            ORDER BY t.created DESC;            
            """,
            new
            {
                team_id = teamId,
                from = from.UtcDateTime
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<TaskForReviewHistory>(command);

        return results.ToArray();
    }
    
    public async Task<IReadOnlyCollection<ReviewTicket>> GetLastFirstReviewers(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT DISTINCT ON (t.owner_id)
                NULLIF(t.first_reviewer_id, t.reviewer_id) AS reviewerid,
            	t.owner_id AS ownerid,
            	t.created AS created
            FROM review.task_for_reviews AS t
            WHERE t.team_id = @team_id AND t.strategy != @nrt_target
            ORDER BY t.owner_id, t.created DESC;
            """,
            new
            {
                team_id = teamId,
                nrt_target = (int)NextReviewerType.Target
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<ReviewTicket>(command);

        return results.ToArray();
    }
    
    public async Task<long?> GetLastSecondReviewer(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT NULLIF(t.second_reviewer_id, t.reviewer_id) AS reviewerid,
            FROM review.task_for_reviews AS t
            WHERE t.team_id = @team_id AND t.first_reviewer_id IS NOT NULL
            ORDER BY t.created DESC;
            """,
            new
            {
                team_id = teamId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var result = await connection.QuerySingleOrDefaultAsync(command);
        
        return result;
    }
}