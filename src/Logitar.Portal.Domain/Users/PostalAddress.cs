using FluentValidation;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users.Validators;

namespace Logitar.Portal.Domain.Users;

public record PostalAddress : IPostalAddress
{
  public PostalAddress(string street, string locality, string country, string? region = null, string? postalCode = null, bool isVerified = false)
  {
    Street = street.Trim();
    Locality = locality.Trim();
    Region = region?.CleanTrim();
    PostalCode = postalCode?.CleanTrim();
    Country = country.Trim();
    IsVerified = isVerified;

    new PostalAddressValidator().ValidateAndThrow(this);
  }

  public string Street { get; }
  public string Locality { get; }
  public string? Region { get; }
  public string? PostalCode { get; }
  public string Country { get; }
  public bool IsVerified { get; }

  public string Format()
  {
    StringBuilder formatted = new();

    string[] lines = Street.Remove("\r").Split('\n');
    foreach (string line in lines)
    {
      formatted.AppendLine(line);
    }

    formatted.Append(Locality);
    if (Region != null)
    {
      formatted.Append(' ').Append(Region);
    }
    if (PostalCode != null)
    {
      formatted.Append(' ').Append(PostalCode);
    }
    formatted.AppendLine();

    formatted.Append(Country);

    return formatted.ToString();
  }
}
