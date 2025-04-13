namespace Recommendations.Abstractions.Messages;
public record BookDeletedMessage
{
    public int BookId { get; set; }
};
