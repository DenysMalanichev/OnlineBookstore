namespace OnlineBookstore.Application.Messages;
public record UserUpsertMessage
{
    public string UserId { get; set; } = default!;
    public IList<int> PurchasedBooks { get; set; } = [];
    public IList<string> PreferedLanguages { get; set; } = [];
    public IList<int> PreferedGenreIds { get; set; } = [];
    public IList<int> PreferedAuthoreIds { get; set; } = [];
    public bool IsPaperbackPrefered { get; set; } = false;
}
