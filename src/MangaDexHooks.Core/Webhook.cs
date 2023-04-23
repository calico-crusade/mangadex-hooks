namespace MangaDexHooks.Core;

public class Webhook
{
	[JsonPropertyName("content")]
	public string Content { get; set; } = string.Empty;

	[JsonPropertyName("username")]
	public string? Username { get; set; }

	[JsonPropertyName("avatarUrl")]
	public string? AvatarUrl { get; set; }

	[JsonPropertyName("tts")]
	public bool Tts { get; set; } = false;

	[JsonPropertyName("threadName")]
	public string? ThreadName { get; set; }

	[JsonPropertyName("threadId")]
	public ulong? ThreadId { get; set; }

	[JsonPropertyName("suppressEmbeds")]
	public bool SuppressEmbeds { get; set; }

	[JsonPropertyName("embeds")]
	public List<WebhookEmbed> Embeds { get; set; } = new();

	public Embed[] ConvertEmbeds() => Embeds.Select(t => (Embed)t).ToArray();

	public class WebhookEmbed
	{
		[JsonPropertyName("type")]
		public EmbedType Type { get; set; } = EmbedType.Rich;

		[JsonPropertyName("description")]
		public string? Description { get; set; }

		[JsonPropertyName("url")]
		public string? Url { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("timestamp")]
		public DateTimeOffset? Timestamp { get; set; }

		[JsonPropertyName("color")]
		public uint? Color { get; set; }

		[JsonPropertyName("image")]
		public string? Image { get; set; }

		[JsonPropertyName("author")]
		public WebhookEmbedAuthor? Author { get; set; }

		[JsonPropertyName("footer")]
		public WebhookEmbedFooter? Footer { get; set; }

		[JsonPropertyName("thumbnail")]
		public string? Thumbnail { get; set; }

		[JsonPropertyName("fields")]
		public List<WebhookEmbedField> Fields { get; set; } = new();

		public static implicit operator Embed(WebhookEmbed embed)
		{
			var builder = new EmbedBuilder()
				.WithTitle(embed.Title)
				.WithDescription(embed.Description)
				.WithUrl(embed.Url)
				.WithImageUrl(embed.Image)
				.WithThumbnailUrl(embed.Thumbnail)
				.WithAuthor(embed.Author?.Name, embed.Author?.IconUrl, embed.Author?.Url)
				.WithFooter(embed.Footer?.Text, embed.Footer?.IconUrl);

			if (embed.Color != null)
				builder.WithColor(new Color(embed.Color.Value));

			if (embed.Timestamp != null)
				builder.WithTimestamp(embed.Timestamp.Value);

			foreach (var field in embed.Fields)
				builder.AddField(field.Name, field.Value, field.Inline);

			return builder.Build();
		}
	}

	public class WebhookEmbedAuthor
	{
		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("url")]
		public string? Url { get; set; }

		[JsonPropertyName("iconUrl")]
		public string? IconUrl { get; set; }
	}

	public class WebhookEmbedFooter
	{
		[JsonPropertyName("text")]
		public string? Text { get; set; }

		[JsonPropertyName("iconUrl")]
		public string? IconUrl { get; set; }
	}

	public class WebhookEmbedField
	{
		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("value")]
		public string? Value { get; set; }

		[JsonPropertyName("inline")]
		public bool Inline { get; set; }
	}
}