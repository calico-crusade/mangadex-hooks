namespace MangaDexHooks.Core.Database.Models;

[Table("webhook_results")]
public class DbWebhookResult : DbObject
{
    [JsonPropertyName("webhookId"), Column("webhook_id")]
    public long WebhookId { get; set; }

    [JsonPropertyName("result")]
    public string? Result { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }
}