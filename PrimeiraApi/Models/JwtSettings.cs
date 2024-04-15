namespace PrimeiraApi.Models
{
    public class JwtSettings
    {
        //Token
        public string? keySecurity { get; set; }
        //Quanto tempo expira
        public int ExpirationTime{ get; set; }
        //Quem enviou
        public string? Sender { get; set; }
        //Audience
        public string? Audience { get; set; }
    }
}
