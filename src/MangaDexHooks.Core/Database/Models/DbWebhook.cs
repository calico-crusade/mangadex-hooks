namespace MangaDexHooks.Core.Database.Models;

[Table("webhooks")]
public class DbWebhook : DbObject
{
    [Column(Unique = true)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("ownerId")]
    [Column("owner_id", Unique = true)]
    public long OwnerId { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public WebhookType Type { get; set; }

    [JsonPropertyName("discordData")]
    [Column("discord_data")]
    public Webhook? DiscordData { get; set; }

    [JsonPropertyName("discordScript")]
    [Column("discord_script")]
    public string? DiscordScript { get; set; }
}

public enum WebhookType
{
    Disabled = 0,
    Discord = 1,
    Json = 2,
    Xml = 3,
	DiscordScript = 4,
}
