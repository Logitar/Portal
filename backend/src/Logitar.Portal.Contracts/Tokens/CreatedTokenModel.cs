namespace Logitar.Portal.Contracts.Tokens;

public record CreatedTokenModel
{
  public string Token { get; set; }

  public CreatedTokenModel() : this(string.Empty)
  {
  }
  public CreatedTokenModel(string token)
  {
    Token = token;
  }
}
