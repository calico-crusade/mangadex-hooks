namespace MangaDexHooks.Api;

public class ShimResult
{
	[JsonPropertyName("shim")]
	public string Shim { get; set; } = string.Empty;

	[JsonPropertyName("default")]
	public string Default { get; set; } = string.Empty;
}
