namespace MangaDexHooks.Core.Database.Models;

public abstract class DbObject
{
    [JsonPropertyName("id")]
    [Column(PrimaryKey = true, ExcludeInserts = true, ExcludeUpdates = true)]
    public virtual long Id { get; set; }

    [JsonPropertyName("createdAt")]
    [Column("created_at", ExcludeInserts = true, ExcludeUpdates = true, OverrideValue = "CURRENT_TIMESTAMP")]
    public virtual DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    [Column("updated_at", ExcludeInserts = true, OverrideValue = "CURRENT_TIMESTAMP")]
    public virtual DateTime UpdatedAt { get; set; }

    [JsonPropertyName("deletedAt")]
    [Column("deleted_at")]
    public virtual DateTime? DeletedAt { get; set; }
}
