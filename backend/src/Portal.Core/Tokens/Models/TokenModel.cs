namespace Portal.Core.Tokens.Models
{
  public class TokenModel
  {
    public TokenModel(string token)
    {
      Token = token ?? throw new ArgumentNullException(nameof(token));
    }

    public string Token { get; }
  }
}
