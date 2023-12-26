namespace Messager.Services.Authenticate;

public class AuthenticateResponse : BaseResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }

}