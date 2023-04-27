using System.Net;

namespace MangaDexHooks.Core;

public static class Extensions
{
	public static string? JsonSerialize<T>(this T item, bool indented = true)
	{
		return JsonSerializer.Serialize(item, new JsonSerializerOptions
		{
			WriteIndented = indented
		});
	}

	public static T? JsonDeserialize<T>(this string? value)
	{
		if (string.IsNullOrEmpty(value)) return default;

		return JsonSerializer.Deserialize<T>(value);
	}

	public static string? UrlEncode(this string? value)
	{
		return WebUtility.UrlEncode(value);
	}

	public static string? HtmlEncode(this string? value)
	{
		return WebUtility.HtmlEncode(value);
	}

	public static string? PreferEn(this Localization locale)
	{
		var en = locale.ContainsKey("en") ? locale["en"] : null;
		if (en != null) return en;

		return locale.FirstOrDefault().Value;
	}

	public static async Task<string?> ReadStaticFile(params string[] path)
	{
		var fullPath = Path.Combine(path.Prepend(AppDomain.CurrentDomain.BaseDirectory).ToArray());
		if (!File.Exists(fullPath)) return null;
		return await File.ReadAllTextAsync(fullPath);
	}
}
