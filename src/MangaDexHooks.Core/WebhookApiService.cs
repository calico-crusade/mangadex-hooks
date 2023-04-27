using CardboardBox.Discord;

namespace MangaDexHooks.Core;

using Database;
using Database.Models;
using MangaDexSharp;

public interface IWebhookApiService
{
	Task<(string msg, int code)> Execute(DbWebhook hook, string chapterId);

	Task<(string msg, int code)> Execute(DbWebhook hook, Chapter chapter);

	Task<(string msg, int code)> Execute(DbWebhook hook, Chapter chapter, Manga manga, string? cover);

	Task<ulong> Execute(string url, Webhook hook);

	Task<(Webhook? result, string? error, int code)> ExecuteWebhookScript(string? script, Chapter chapter, Manga manga, string? cover);

	Task<(Manga? manga, Chapter? chapter, string? cover)> GetData(string chapterId);
}

public class WebhookApiService : IWebhookApiService
{
	private static string? _script;

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

	public async Task<(Manga? manga, Chapter? chapter, string? cover)> GetData(string chapterId)
	{
		var chapter = await _md.Chapter.Get(chapterId);
		if (chapter == null || chapter.Data == null) return (null, null, null);

		var (manga, cover) = await FillData(chapter.Data);
		return (manga, chapter.Data, cover);
	}

	public async Task<(Manga? manga, string? cover)> FillData(Chapter chapter)
	{
		var manga = chapter.Manga().FirstOrDefault();
		if (manga == null) return (null, null);

		var cover = (await _md.Cover.List(new()
		{
			MangaIds = new[] { manga.Id },
			Order =
			{
				[CoverArtFilter.OrderKey.volume] = OrderValue.desc
			}
		}))?.Data.FirstOrDefault()?.Attributes?.FileName;
		var coverUrl = cover != null ? $"https://mangadex.org/covers/{manga.Id}/{cover}" : null;
		return (manga, coverUrl);
	}

	public async Task<(string msg, int code)> Execute(DbWebhook hook, Chapter chapter)
	{
		var (manga, coverUrl) = await FillData(chapter);
		if (manga == null)
			return ("Couldn't determine manga from chapter. Are you missing includes?", 404);

		return await Execute(hook, chapter, manga, coverUrl);
	}

	public async Task<(string msg, int code)> Execute(DbWebhook hook, Chapter chapter, Manga manga, string? cover)
	{
		try
		{
			var (msg, code) = hook.Type switch
			{
				WebhookType.Json => await ExecuteJson(hook.Url, chapter),
				WebhookType.Discord => await ExecuteDiscord(hook.Url, hook.DiscordData, chapter, manga, cover),
				WebhookType.Disabled => ("Webhook disabled - I didn't do anything.", 200),
				WebhookType.DiscordScript => await ExecuteDiscordScript(hook.Url, hook.DiscordScript, chapter, manga, cover),
				_ => ("Invalid Webhook Type", 500)
			};

			var result = new DbWebhookResult
			{
				WebhookId = hook.Id,
				Code = code,
				Results = $"{(code >= 200 && code < 300 ? "Success!" : "Something went wrong! Response code does not indicate success.")}\r\n```\r\n{msg}\r\n```"
			};

			await _db.Results.Insert(result);

			return (msg, code);
		}
		catch (Exception ex)
		{
			var result = new DbWebhookResult
			{
				WebhookId = hook.Id,
				Code = 500,
				Results = $"Something went wrong! Response code does not indicate success.\r\n```\r\n{ex}\r\n```"
			};

			await _db.Results.Insert(result);
			return (result.Results, result.Code);
		}
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

	public async Task<(string msg, int code)> ExecuteDiscord(string url, Webhook? hook, Chapter chapter, Manga manga, string? cover)
	{
		hook ??= new()
		{
			Content = "Check out this chapter!"
		};

		hook.Embeds.Add(new Webhook.WebhookEmbed
		{
			Title = manga.Attributes.Title.PreferEn(),
			Description = manga.Attributes.Description.PreferEn(),
			Thumbnail = cover,
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

	public async Task<string?> LoadScript()
	{
		return _script ??= await Extensions.ReadStaticFile("Scripts", "BaseMangaTemplate.js");
	}

	public async Task<(string msg, int code)> ExecuteDiscordScript(string url, string? script, Chapter chapter, Manga manga, string? cover)
	{
		var (hook, error, code) = await ExecuteWebhookScript(script, chapter, manga, cover);
		if (hook == null) return (error ?? "An error occurred while trying to run your script", code);

		var res = await Execute(url, hook);
		return ("Results UID: " + res, 200);
	}

	public async Task<(Webhook? result, string? error, int code)> ExecuteWebhookScript(string? script, Chapter chapter, Manga manga, string? cover)
	{
		if (string.IsNullOrEmpty(script))
			return (null, "You didn't specify a script to run.", 500);

		const string INSERT_REPLACEMENT = "/*INSERT SCRIPT HERE*/";
		var baseScript = await LoadScript();
		if (string.IsNullOrEmpty(baseScript))
			return (null, "I couldn't execute the script: Missing runner.", 500);

		var actualScript = baseScript.Replace(INSERT_REPLACEMENT, script);

		using var runner = new ScriptRunner(actualScript);

		var result = runner.Eval(manga, chapter, cover);
		if (result == null) return (null, "Executed script did not return anything?", 500);

		var hook = result.JsonDeserialize<Webhook>();
		if (hook == null) return (null, "Couldn't parse result of script.", 500);

		return (hook, null, 200);
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
