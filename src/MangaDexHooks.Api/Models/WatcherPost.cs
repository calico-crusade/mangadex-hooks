namespace MangaDexHooks.Api;

public class WatcherPost
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	[JsonPropertyName("type")]
	public ResourceType Type { get; set; }

	public void Deconstruct(out string id, out ResourceType type)
	{
		id = Id;
		type = Type;
	}
}
