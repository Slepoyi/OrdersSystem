namespace OrdersSystem.Api.Options
{
    public class JwtOptions
    {
        public const string Section = "JwtOptions";

        public TimeSpan ClockSkew { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public string ValidAudience { get; set; } = String.Empty;
        public string ValidIssuer { get; set; } = String.Empty;
        public string Secret { get; set; } = String.Empty;
    }
}
