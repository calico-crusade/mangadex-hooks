namespace MangaDexHooks.Core.Database.Models;

[Table("mangadex_cache")]
public class DbMdCache : DbObject
{
	[JsonPropertyName("resourceId"), Column("resource_id", Unique = true)]
	public string ResourceId { get; set; } = string.Empty;

	[JsonPropertyName("type"), Column("resource_type", Unique = true)]
	public ResourceType ResourceType { get; set; }

	[JsonPropertyName("results")]
	public string Results { get; set; } = string.Empty;

	[JsonPropertyName("lastCheck"), Column("last_check")]
	public DateTime? LastCheck { get; set;  }
}

public enum ResourceType
{
	Manga = 1,
	User = 2,
	Group = 3,
	CustomList = 4,
}
