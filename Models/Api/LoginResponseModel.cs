namespace BlackBank.Models.Api
{
    public class LoginResponseModel
    {
        public string? Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
