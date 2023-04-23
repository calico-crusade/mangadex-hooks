namespace MangaDexHooks.Core.Database.Models;

[Table("webhook_results")]
public class DbWebhookResult : DbObject
{
    [JsonPropertyName("webhookId"), Column("webhook_id")]
    public long WebhookId { get; set; }

    [JsonPropertyName("results")]
    public string? Results { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }
}