namespace SecurityGateway
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int TokenExpirationMinutes { get; set; }  // Ejemplo: 60
        public int TokenInactivityMinutes { get; set; }  // Ejemplo: 3 minutos para inactividad
    }
}
