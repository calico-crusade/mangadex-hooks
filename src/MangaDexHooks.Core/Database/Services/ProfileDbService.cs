namespace MangaDexHooks.Core.Database.Services;

using Models;

public interface IProfileDbService
{
	Task<int> Upsert(DbProfile profile, bool fake = true);

	Task<PaginatedResult<DbProfile>> Paginate(int page = 1, int size = 100);

	Task<DbProfile?> Fetch(long id);

	Task<DbProfile?> Fetch(string platformId);
}

public class ProfileDbService : OrmMap<DbProfile>, IProfileDbService
{
	private static string? _queryFetchPlatform;

	public ProfileDbService(
		ISqlService sql, 
		IQueryService query, 
		IFakeUpsertQueryService fake) : base(query, sql, fake) { }

	public Task<DbProfile?> Fetch(string platformId)
	{
		_queryFetchPlatform ??= _query.Select<DbProfile>(t => t.With(a => a.PlatformId));
		return _sql.Fetch<DbProfile?>(_queryFetchPlatform, new { PlatformId = platformId });
	}
}
