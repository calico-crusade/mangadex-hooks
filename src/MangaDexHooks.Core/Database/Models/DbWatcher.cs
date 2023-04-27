namespace MangaDexHooks.Core.Database.Models;

[Table("watchers")]
public class DbWatcher : DbObject
{
	[JsonPropertyName("itemId"), Column("item_id", Unique = true)]
	public string ItemId { get; set; } = string.Empty;

	[JsonPropertyName("type"), Column("watch_type", Unique = true)]
	public ResourceType WatchType { get; set; }

	[JsonPropertyName("webhookId"), Column("webhook_id", Unique = true)]
	public long WebhookId { get; set; }

	[JsonPropertyName("resourceName"), Column("resource_name")]
	public string ResourceName { get; set; } = string.Empty;

	[JsonPropertyName("resourceImage"), Column("resource_image")]
	public string ResourceImage { get; set; } = string.Empty;

	[JsonPropertyName("cacheItems"), Column("cache_items")]
	public string[] CacheItems { get; set; } = Array.Empty<string>();

	[JsonPropertyName("lastCacheCheck"), Column("last_cache_check")]
	public DateTime? LastCacheCheck { get; set; }
}