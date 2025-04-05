namespace Recommendations.Abstractions.Entities;
public record UserPortrait
{
    public string UserId { get; set; } = default!;
    public IList<int> PreferedLanguageIds { get; set; } = [];
    public IList<int> PreferedGenreIds { get; set; } = [];
    public IList<int> PreferedAuthoreIds { get; set; } = [];
    public bool IsPaperbackPrefered { get; set; } = false;
}
