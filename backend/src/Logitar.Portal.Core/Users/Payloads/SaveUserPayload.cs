namespace Logitar.Portal.Core.Users.Payloads
{
  public abstract class SaveUserPayload
  {
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }

    public string? Locale { get; set; }
    public string? Picture { get; set; }
  }
}
