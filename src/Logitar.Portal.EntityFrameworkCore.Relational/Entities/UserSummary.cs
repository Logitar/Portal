using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record UserSummary
{
  public Guid Id { get; init; }
  public string EmailAddress { get; init; } = string.Empty;
  public string? FullName { get; init; }
  public string? Picture { get; init; }

  internal static UserSummary From(UserEntity user) => new()
  {
    Id = new AggregateId(user.AggregateId).ToGuid(),
    EmailAddress = user.EmailAddress ?? string.Empty,
    FullName = user.FullName,
    Picture = user.Picture
  };
}
