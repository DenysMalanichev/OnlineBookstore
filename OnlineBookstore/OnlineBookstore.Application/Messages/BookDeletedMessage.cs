namespace OnlineBookstore.Application.Messages
{
    public record BookDeletedMessage
    {
        public int BookId { get; set; }
    }
}
