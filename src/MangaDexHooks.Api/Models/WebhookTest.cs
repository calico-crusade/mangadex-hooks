namespace MangaDexHooks.Api;

public class WebhookTest
{
	[JsonPropertyName("script")]
	public string Script { get; set; } = string.Empty;

	[JsonPropertyName("chapterId")]
	public string? ChapterId { get; set; }
}
