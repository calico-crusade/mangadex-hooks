namespace MangaDexHooks.Core.Database.Services;

using Models;

public interface IWebhookDbService
{
	Task<DbWebhook?> Fetch(long id);

	Task<int> Upsert(DbWebhook item, bool fake = true);

	Task<PaginatedResult<DbWebhook>> ByOwner(long ownerId);

	Task<int> Delete(long id);
}

public class WebhookDbService : OrmMap<DbWebhook>, IWebhookDbService
{
	private static string? _queryOwner;

	public WebhookDbService(
		ISqlService sql,
		IQueryService query,
		IFakeUpsertQueryService fake) : base(query, sql, fake) { }

	public Task<PaginatedResult<DbWebhook>> ByOwner(long ownerId)
	{
		_queryOwner ??= _query.Paginate<DbWebhook, DateTime>(
			t => t.CreatedAt, false,
			t => t.With(a => a.OwnerId));

		return _sql.Paginate<DbWebhook>(_queryOwner, new { OwnerId = ownerId });
	}
}
