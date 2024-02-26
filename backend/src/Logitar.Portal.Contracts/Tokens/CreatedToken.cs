namespace Logitar.Portal.Contracts.Tokens;

public record CreatedToken
{
  public string Token { get; set; }

  public CreatedToken() : this(string.Empty)
  {
  }
  public CreatedToken(string token)
  {
    Token = token;
  }
}
