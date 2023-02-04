using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Infrastructure.Entities;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class SenderProfile : Profile
  {
    public SenderProfile()
    {
      CreateMap<SenderEntity, SenderModel>()
        .IncludeBase<AggregateEntity, AggregateModel>()
        .ForMember(x => x.Settings, x => x.MapFrom(GetSettings));
    }

    private static IEnumerable<SettingModel> GetSettings(SenderEntity sender, SenderModel model)
    {
      if (sender.Settings == null)
      {
        return Enumerable.Empty<SettingModel>();
      }

      Dictionary<string, string?> settings = JsonSerializer.Deserialize<Dictionary<string, string?>>(sender.Settings)
        ?? throw new InvalidOperationException($"The sender 'Id={sender.SenderId}' settings could not be deserialized.");

      return settings.Select(setting => new SettingModel
      {
        Key = setting.Key,
        Value = setting.Value
      });
    }
  }
}
