namespace OnlineBookstore.Features.UserFeatures.Options;

public class JwtOptions
{
    public string Secret { get; set; } = default!;

    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;
}