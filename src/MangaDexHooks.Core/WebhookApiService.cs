using CardboardBox.Discord;

namespace MangaDexHooks.Core;

using Database;
using Database.Models;

public interface IWebhookApiService
{
	Task<(string msg, int code)> Execute(DbWebhook hook, string chapterId);

	Task<(string msg, int code)> Execute(DbWebhook hook, Chapter chapter);

	Task<ulong> Execute(string url, Webhook hook);
}

public class WebhookApiService : IWebhookApiService
{
	private readonly IApiService _api;
	private readonly IMangaDex _md;
	private readonly IDbService _db;

	public WebhookApiService(
		IApiService api, 
		IMangaDex md,
		IDbService db)
	{
		_api = api;
		_md = md;
		_db = db;
	}

	public static DiscordWebhookClient GetClient(string url) => new(url);

	public async Task<(string msg, int code)> Execute(DbWebhook hook, string chapterId)
	{
		var chapter = await _md.Chapter.Get(chapterId);
		if (chapter == null || chapter.Data == null) return ("Couldn't find chapter", 404);

		return await Execute(hook, chapter.Data);
	}

	public async Task<(string msg, int code)> Execute(DbWebhook hook, Chapter chapter)
	{
		var (msg, code) = hook.Type switch
		{
			WebhookType.Json => await ExecuteJson(hook.Url, chapter),
			WebhookType.Discord => await ExecuteDiscord(hook.Url, hook.DiscordData, chapter),
			_ => ("Invalid Webhook Type", 500)
		};

		var result = new DbWebhookResult
		{
			WebhookId = hook.Id,
			Code = code,
			Results = msg
		};

		await _db.Results.Insert(result);

		return (msg, code);
	}

	public async Task<(string msg, int code)> ExecuteJson<T>(string url, T data)
	{
		var resp = await _api.Create(url, "POST")
			.Body(data)
			.Result();

		if (resp == null) return ("Response was empty", 500);

		var code = (int)resp.StatusCode;
		var content = await resp.Content.ReadAsStringAsync();

		return (content, code);
	}

	public async Task<(string msg, int code)> ExecuteDiscord(string url, Webhook? hook, Chapter chapter)
	{
		hook ??= new()
		{
			Content = "Check out this chapter!"
		};

		var manga = chapter.Manga().FirstOrDefault();
		if (manga == null)
			return ("Couldn't determine manga from chapter. Are you missing includes?", 404);

		var cover = (await _md.Cover.List(new()
		{
			MangaIds = new[] { manga.Id },
			Order =
			{
				[CoverArtFilter.OrderKey.volume] = OrderValue.desc
			}
		}))?.Data.FirstOrDefault()?.Attributes?.FileName;
		var coverUrl = cover != null ? $"https://mangadex.org/covers/{manga.Id}/{cover}" : null;

		Console.WriteLine("Cover: " + coverUrl);

		hook.Embeds.Add(new Webhook.WebhookEmbed
		{
			Title = manga.Attributes.Title.PreferEn(),
			Description = manga.Attributes.Description.PreferEn(),
			Thumbnail = coverUrl,
			Url = $"https://mangadex.org/chapter/{chapter.Id}",
			Fields = new()
			{
				new()
				{
					Name = "Tags",
					Value = string.Join(", ", manga.Attributes.Tags.Select(t => t.Attributes.Name.PreferEn()))
				},
				new()
				{
					Name = "Manga",
					Value = $"[mangadex](https://mangadex.org/title/{manga.Id})",
					Inline = true
				},
				new()
				{
					Name = "Rating",
					Value = manga.Attributes.ContentRating.ToString(),
					Inline = true
				},
				new()
				{
					Name = "Status",
					Value = $"{manga.Attributes.State} - {manga.Attributes.Status}",
					Inline = true
				},
				new()
				{
					Name = "Demographic",
					Value = manga.Attributes.PublicationDemographic.ToString(),
					Inline = true
				},
				new()
				{
					Name = "Updated At",
					Value = manga.Attributes.UpdatedAt.GenerateTimestamp(),
					Inline = true
				},
				new()
				{
					Name = "Readable At",
					Value = chapter.Attributes.ReadableAt.GenerateTimestamp(),
					Inline = true
				}
			}
		});

		var res = await Execute(url, hook);
		return ("Results UID: " + res, 200);
	}

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
