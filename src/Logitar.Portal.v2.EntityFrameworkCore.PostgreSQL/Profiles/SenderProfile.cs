using AutoMapper;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Senders;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using System.Text.Json;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Profiles;

internal class SenderProfile : Profile
{
  public SenderProfile()
  {
    CreateMap<SenderEntity, Sender>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.GetId))
      .ForMember(x => x.Settings, x => x.MapFrom(GetSettings));
  }

  private static IEnumerable<Setting> GetSettings(SenderEntity sender, Sender _)
  {
    if (sender.Settings == null)
    {
      return Enumerable.Empty<Setting>();
    }

    Dictionary<string, string> settings = JsonSerializer.Deserialize<Dictionary<string, string>>(sender.Settings)
      ?? throw new InvalidOperationException($"The settings deserialization failed: '{sender.Settings}'.");

    return settings.Select(pair => new Setting
    {
      Key = pair.Key,
      Value = pair.Value
    });
  }
}
