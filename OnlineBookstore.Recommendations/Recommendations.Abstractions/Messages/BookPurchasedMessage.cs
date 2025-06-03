namespace Recommendations.Abstractions.Messages;
public record BookPurchasedMessage
{
    public int BookId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
