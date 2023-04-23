namespace MangaDexHooks.Core.Database.Services;

using Models;

public interface IWebhookResultsDbService
{
	Task<DbWebhookResult?> Fetch(long id);

	Task<int> Insert(DbWebhookResult results);

	Task<PaginatedResult<DbWebhookResult>> ByWebhook(long id, int page, int size);
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
}
