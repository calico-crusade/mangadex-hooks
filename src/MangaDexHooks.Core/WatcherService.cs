namespace MangaDexHooks.Core;

using Database;
using Database.Models;
using MangaDexSharp;

public interface IWatcherService
{
	Task<CodeResult> Upsert(string resource, ResourceType type, long hook);
}

public class WatcherService : IWatcherService
{
	private readonly IDbService _db;
	private readonly IMdCacheService _md;
	private readonly ILogger _logger;

	public WatcherService(
		IDbService db, 
		IMdCacheService md, 
		ILogger<WatcherService> logger)
	{
		_db = db;
		_md = md;
		_logger = logger;
	}

	public async Task<CodeResult> Upsert(string id, ResourceType type, long hook, string name, string cover, params string[] cache)
	{
		var item = new DbWatcher
		{
			ItemId = id,
			WatchType = type,
			WebhookId = hook,
			ResourceImage = cover,
			ResourceName = name,
			CacheItems = cache,
			LastCacheCheck = DateTime.Now,
		};

		var outid = await _db.Watchers.Upsert(item);
		return new(200, "Resource Added", outid);
	}

	public async Task<CodeResult> Upsert(string resource, ResourceType type, long hook)
	{
		try
		{
			return type switch
			{
				ResourceType.Manga => await Manga(resource, hook),
				ResourceType.Group => await Group(resource, hook),
				ResourceType.CustomList => await List(resource, hook),
				ResourceType.User => await User(resource, hook),
				_ => new CodeResult(400, "Couldn't determine the type of resource", null)
			};
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while processing hook: {hook} >> {type}: {resource}", hook, type, resource);
			return new(500, ex.Message, null);
		}
	}

	public async Task<CodeResult> Manga(string id, long hook)
	{
		var manga = await _md.Manga(id);
		if (manga == null) return new(404, "Couldn't find that manga", null);

		var name = manga.Data.Attributes.Title.PreferEn();
		if (string.IsNullOrEmpty(name)) return new(400, "Manga didn't return a title!", null);

		var cover = manga.Data.CoverArt().FirstOrDefault()?.Attributes.FileName;
		var url = string.IsNullOrEmpty(cover) ? "" : $"https://mangadex.org/covers/{manga.Data.Id}/{cover}";
		return await Upsert(id, ResourceType.Manga, hook, name, url);
	}

	public async Task<CodeResult> Group(string id, long hook)
	{
		var group = await _md.Group(id);
		if (group == null) return new(404, "Couldn't find that group", null);

		var name = group.Data.Attributes.Name;
		if (string.IsNullOrEmpty(name)) return new(400, "Group didn't return a title!", null);

		var cover = "https://mangadex.org/img/avatar.png";
		return await Upsert(id, ResourceType.Group, hook, name, cover);
	}

	public async Task<CodeResult> List(string id, long hook)
	{
		var list = await _md.List(id);
		if (list == null) return new(404, "Coudln't find that list", null);

		var name = list.Data.Attributes.Name;
		if (string.IsNullOrEmpty(name)) return new(400, "List didn't return a title!", null);

		var ids = list.Data.Relationships
			.Where(t => t is RelatedDataRelationship)
			.Select(t => ((RelatedDataRelationship)t).Id)
			.ToArray();

		if (ids.Length == 0) return new(400, "List contains no manga IDs", null);

		var mdId = ids.First();
		var coverMd = await _md.Manga(mdId);
		var cover = coverMd?.Data.CoverArt().FirstOrDefault()?.Attributes.FileName;
		var url = string.IsNullOrEmpty(cover) ? "" : $"https://mangadex.org/covers/{mdId}/{cover}";

		return await Upsert(id, ResourceType.CustomList, hook, name, url, ids);
	}

	public async Task<CodeResult> User(string id, long hook)
	{
		var user = await _md.User(id);
		if (user == null) return new(404, "Couldn't find that group", null);

		var name = user.Data.Attributes.Username;
		if (string.IsNullOrEmpty(name)) return new(400, "User didn't return a username!", null);

		var cover = "https://mangadex.org/img/avatar.png";
		return await Upsert(id, ResourceType.User, hook, name, cover);
	}
}

public record class CodeResult(int Code, string? Message, long? Id);
