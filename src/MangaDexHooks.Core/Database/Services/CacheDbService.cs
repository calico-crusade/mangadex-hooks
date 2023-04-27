namespace MangaDexHooks.Core.Database.Services;

using Models;

public interface ICacheDbService
{
	Task<long> Upsert(DbMdCache cache);

	Task<DbMdCache?> Fetch(string id, ResourceType type);
}

public class CacheDbService : OrmMap<DbMdCache>, ICacheDbService
{
	private static string? _queryFetch;

	public CacheDbService(
		ISqlService sql,
		IQueryService query,
		IFakeUpsertQueryService fake) : base(query, sql, fake) { }

	public Task<DbMdCache?> Fetch(string id, ResourceType type)
	{
		_queryFetch ??= _query.Select<DbMdCache>(t => t.With(a => a.ResourceId).With(a => a.ResourceType));
		return _sql.Fetch<DbMdCache>(_queryFetch, new { ResourceId = id, ResourceType = type });
	}
}
