namespace Logitar.Portal.Contracts.Users;

public interface IPostalAddress
{
  string Street { get; }
  string Locality { get; }
  string? Region { get; }
  string? PostalCode { get; }
  string Country { get; }
}
