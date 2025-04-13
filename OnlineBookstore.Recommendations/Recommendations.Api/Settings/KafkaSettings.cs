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
    public string BookDeletedTopic { get; set; } = "book-deleted";

    /// <summary>
    /// The topic for book updated messages
    /// </summary>
    public string BookUpdatedTopic { get; set; } = "book-updated";

    /// <summary>
    /// The topic for user updated messages
    /// </summary>
    public string UserUpdatedTopic { get; set; } = "user-updated";
}
