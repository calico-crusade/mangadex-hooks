namespace MangaDexHooks.Core.Database;

using Services;

public interface IDbService
{
	IWebhookDbService Webhooks { get; }

	IProfileDbService Profiles { get; }

	IWebhookResultsDbService Results { get; }

	ICacheDbService Cache { get; }

	IWatcherDbService Watchers { get; }
}

public class DbService : IDbService
{
	public IWebhookDbService Webhooks { get; }

	public IProfileDbService Profiles { get; }

	public IWebhookResultsDbService Results { get; }

	public ICacheDbService Cache { get; }

	public IWatcherDbService Watchers { get; }

	public DbService(
		IWebhookDbService webhooks, 
		IProfileDbService profiles,
		IWebhookResultsDbService results,
		ICacheDbService cache,
		IWatcherDbService watchers)
	{
		Webhooks = webhooks;
		Profiles = profiles;
		Results = results;
		Cache = cache;
		Watchers = watchers;
	}
}
