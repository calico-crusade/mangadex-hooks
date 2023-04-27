namespace MangaDexHooks.Core.Database.Models;

public class ResultsWithHook
{
	[JsonPropertyName("hook")]
	public DbWebhook Hook { get; set; } = new();

	[JsonPropertyName("result")]
	public DbWebhookResult Result { get; set; } = new();

	public ResultsWithHook() { }

	public ResultsWithHook(DbWebhook hook, DbWebhookResult result)
	{
		Hook = hook;
		Result = result;
	}
}
