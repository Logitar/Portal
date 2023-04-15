namespace Logitar.Portal.Core.Tokens.Models
{
  public class ValidatedTokenModel
  {
    public IEnumerable<ErrorModel> Errors { get; set; } = Enumerable.Empty<ErrorModel>();
    public bool Succeeded => !Errors.Any();

    public string? Email { get; set; }
    public string? Subject { get; set; }

    public IEnumerable<ClaimModel>? Claims { get; set; }
  }
}
