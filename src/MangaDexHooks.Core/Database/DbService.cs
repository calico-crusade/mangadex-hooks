namespace MangaDexHooks.Core.Database;

using Services;

public interface IDbService
{
	IWebhookDbService Webhooks { get; }

	IProfileDbService Profiles { get; }

	IWebhookResultsDbService Results { get; }
}

public class DbService : IDbService
{
	public IWebhookDbService Webhooks { get; }

	public IProfileDbService Profiles { get; }

	public IWebhookResultsDbService Results { get; }

	public DbService(
		IWebhookDbService webhooks, 
		IProfileDbService profiles,
		IWebhookResultsDbService results)
	{
		Webhooks = webhooks;
		Profiles = profiles;
		Results = results;
	}
}
