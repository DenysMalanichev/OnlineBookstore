namespace Recommendations.Api.Settings;
/// <summary>
/// Settings for Kafka configuration
/// </summary>
public class KafkaSettings
{
    /// <summary>
    /// The Kafka bootstrap servers
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>
    /// The consumer group ID
    /// </summary>
    public string GroupId { get; set; } = "recommendations-group";

    /// <summary>
    /// The topic for book deleted messages
    /// </summary>
    public string BookDeletedTopic { get; set; } = "recommendations.book-deleted";

    /// <summary>
    /// The topic for book upserted messages
    /// </summary>
    public string BookUpsertedTopic { get; set; } = "recommendations.book-upserted";

    /// <summary>
    /// The topic for book purchased messages
    /// </summary>
    public string BookPurchasedTopic { get; set; } = "recommendations.book-purchased";

    /// <summary>
    /// The topic for user updated messages
    /// </summary>
    public string UserUpsertedTopic { get; set; } = "recommendations.user-upserted";
}
