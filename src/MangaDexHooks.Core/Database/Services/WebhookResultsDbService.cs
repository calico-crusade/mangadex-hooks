namespace MangaDexHooks.Core.Database.Services;

using Models;

public interface IWebhookResultsDbService
{
	Task<DbWebhookResult?> Fetch(long id);

	Task<int> Insert(DbWebhookResult results);

	Task<PaginatedResult<DbWebhookResult>> ByWebhook(long id, int page, int size);

	Task<PaginatedResult<ResultsWithHook>> ByOwner(long id, int page, int size);
}

public class WebhookResultsDbService : OrmMap<DbWebhookResult>, IWebhookResultsDbService
{
	private static string? _queryWebhook;

	public WebhookResultsDbService(
		ISqlService sql,
		IQueryService query,
		IFakeUpsertQueryService fake) : base(query, sql, fake) { }

	public Task<PaginatedResult<DbWebhookResult>> ByWebhook(long id, int page, int size)
	{
		_queryWebhook ??= _query.Paginate<DbWebhookResult, DateTime>(
			t => t.CreatedAt, false,
			t => t.With(a => a.WebhookId));

		return _sql.Paginate<DbWebhookResult>(_queryWebhook, new { WebhookId = id }, page, size);
	}

	public async Task<PaginatedResult<ResultsWithHook>> ByOwner(long id, int page, int size)
	{
		const string QUERY = @"SELECT h.*, '' as split, r.*
FROM webhooks h
JOIN webhook_results r ON h.id = r.webhook_id
WHERE h.owner_id = :id
ORDER BY r.created_at DESC
LIMIT :limit OFFSET :offset;

SELECT COUNT(*)
FROM webhooks h
JOIN webhook_results r ON h.id = r.webhook_id
WHERE h.owner_id = :id;";

		using var con = await _sql.CreateConnection();
		using var rdr = await con.QueryMultipleAsync(QUERY, new
		{
			id,
			limit = size,
			offset = (page - 1) * size
		});

		var res = rdr.Read<DbWebhook, DbWebhookResult, ResultsWithHook>((h, r) => new(h, r), "split").ToArray();
		var total = await rdr.ReadSingleAsync<int>();

		var pages = (int)Math.Ceiling((double)total / size);
		return new(pages, total, res);
	}
}
