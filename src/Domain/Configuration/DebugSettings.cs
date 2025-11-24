namespace WebApp.Configuration
{
    public class DebugSettings
    {
        public required string SecretKey { get; set; }

        public int TokenExpiryMinutes { get; set; } = 5;

        public required string BaseUrl { get; set; }
    }
}
