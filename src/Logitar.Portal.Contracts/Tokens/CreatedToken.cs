namespace Logitar.Portal.Contracts.Tokens;

public record CreatedToken
{
  public CreatedToken() : this(string.Empty)
  {
  }
  public CreatedToken(string token)
  {
    Token = token;
  }

  public string Token { get; set; }
}
