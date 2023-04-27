namespace MangaDexHooks.Core.Database.Services;

using Models;

public interface IWatcherDbService
{
	Task<PaginatedResult<DbWatcher>> ByWebhook(long id, int page, int size);

	Task<long> Upsert(DbWatcher watcher);

	Task<int> Delete(long id);

	Task<DbWatcher?> Fetch(long id);
}

public class WatcherDbService : OrmMap<DbWatcher>, IWatcherDbService
{
	private static string? _queryWatchers;

	public WatcherDbService(
		ISqlService sql,
		IQueryService query,
		IFakeUpsertQueryService fake) : base(query, sql, fake) { }

	public Task<PaginatedResult<DbWatcher>> ByWebhook(long id, int page, int size)
	{
		_queryWatchers ??= _query.Paginate<DbWatcher, DateTime>(
			t => t.CreatedAt, false,
			t => t.With(a => a.WebhookId));
		return _sql.Paginate<DbWatcher>(_queryWatchers, new { WebhookId = id }, page, size);
	}
}
