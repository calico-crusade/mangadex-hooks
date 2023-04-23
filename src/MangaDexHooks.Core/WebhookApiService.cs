namespace MangaDexHooks.Core;

public interface IWebhookApiService
{
	Task<ulong> Execute(string url, Webhook hook);
}

public class WebhookApiService : IWebhookApiService
{
	public static DiscordWebhookClient GetClient(string url) => new(url);

	public async Task<ulong> Execute(string url, Webhook hook)
	{
		using var client = GetClient(url);
		return await client.SendMessageAsync(hook.Content,
			hook.Tts,
			hook.ConvertEmbeds(),
			hook.Username,
			hook.AvatarUrl,
			null, null, null,
			hook.SuppressEmbeds ? MessageFlags.SuppressEmbeds : MessageFlags.None,
			hook.ThreadId,
			hook.ThreadName);
	}
}
