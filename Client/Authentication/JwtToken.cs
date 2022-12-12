namespace MemeIT.Client.Authentication
{
    public record class JwtToken
    {
        public string Token { get; init; } = string.Empty;

        public DateTime? Expiration { get; init; } = default!;
    }
}
