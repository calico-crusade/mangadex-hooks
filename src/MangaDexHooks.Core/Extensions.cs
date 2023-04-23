using System.Net;

namespace MangaDexHooks.Core;

public static class Extensions
{
	public static string JsonSerialize<T>(this T item, bool indented = true)
	{
		return JsonSerializer.Serialize(item, new JsonSerializerOptions
		{
			WriteIndented = indented
		});
	}

	public static string? UrlEncode(this string? value)
	{
		return WebUtility.UrlEncode(value);
	}

	public static string? HtmlEncode(this string? value)
	{
		return WebUtility.HtmlEncode(value);
	}
}
