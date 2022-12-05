namespace MemeIT.Client.Authentication
{
    public record class JwtToken
    {
        public string Token { get; } = default!;

        public DateTime? Expiration { get; } = default!;
    }
}
