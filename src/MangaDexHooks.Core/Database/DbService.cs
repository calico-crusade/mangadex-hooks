namespace MangaDexHooks.Core.Database;

using Services;

public interface IDbService
{
	IWebhookDbService Webhooks { get; }

	IProfileDbService Profiles { get; }
}

public class DbService : IDbService
{
	public IWebhookDbService Webhooks { get; }

	public IProfileDbService Profiles { get; }

	public DbService(
		IWebhookDbService webhooks, 
		IProfileDbService profiles)
	{
		Webhooks = webhooks;
		Profiles = profiles;
	}
}
