namespace MangaDexHooks.Core;

using Database;
using Database.Models;

public interface IMdCacheService
{
	Task<MangaDexRoot<Manga>?> Manga(string id);

	Task<MangaDexRoot<ScanlationGroup>?> Group(string id);

	Task<MangaDexRoot<CustomList>?> List(string id);

	Task<MangaDexRoot<User>?> User(string id);
}

public class MdCacheService : IMdCacheService
{
	private readonly IMangaDex _md;
	private readonly IDbService _db;
	private readonly ILogger _logger;

	private int _cacheMin => 10;

	public MdCacheService(
		IMangaDex md, 
		IDbService db,
		ILogger<MdCacheService> logger)
	{
		_md = md;
		_db = db;
		_logger = logger;
	}

	public bool Expired(DbMdCache cache)
	{
		var min = DateTime.Now.AddMinutes(-_cacheMin);
		if (cache.LastCheck == null) return true;

		return cache.LastCheck < min;
	}

	public T? JsonDeserialize<T>(string data)
	{
		try
		{
			return data.JsonDeserialize<T>();
		}
		catch (Exception ex)
		{
			var type = typeof(T).Name;
			_logger.LogError(ex, "Couldn't deserialize JSON for: {type} - {data}", type, data);
			return default;
		}
	}

	public async Task<T?> GetCache<T>(string resource, ResourceType type)
	{
		var item = await _db.Cache.Fetch(resource, type);
		return item == null || Expired(item) ? default : JsonDeserialize<T>(item.Results);
	}

	public Task SetCache<T>(T data, string id, ResourceType type)
	{
		var result = data.JsonSerialize(false);
		if (string.IsNullOrEmpty(result)) return Task.CompletedTask;

		return _db.Cache.Upsert(new DbMdCache
		{
			Results = result,
			ResourceId = id,
			ResourceType = type,
			LastCheck = DateTime.Now
		});
	}

	public async Task<T?> Get<T>(string id, ResourceType type, Func<string, Task<T>> result)
	{
		var data = await GetCache<T>(id, type);
		if (data != null) return data;

		data = await result(id);
		if (data == null) return default;

		await SetCache(data, id, type);
		return data;
	}

	public Task<MangaDexRoot<Manga>?> Manga(string id) => Get(id, ResourceType.Manga, (id) => _md.Manga.Get(id));

	public Task<MangaDexRoot<ScanlationGroup>?> Group(string id) => Get(id, ResourceType.Group, _md.ScanlationGroup.Get);

	public Task<MangaDexRoot<CustomList>?> List(string id) => Get(id, ResourceType.CustomList, _md.Lists.Get);

	public Task<MangaDexRoot<User>?> User(string id) => Get(id, ResourceType.User, _md.User.Get);
}
