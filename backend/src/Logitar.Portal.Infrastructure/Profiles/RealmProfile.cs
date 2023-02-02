using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Infrastructure.Entities;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class RealmProfile : Profile
  {
    public RealmProfile()
    {
      CreateMap<RealmEntity, RealmModel>()
        .IncludeBase<AggregateEntity, AggregateModel>()
        .ForMember(x => x.UsernameSettings, x => x.MapFrom(GetUsernameSettings))
        .ForMember(x => x.PasswordSettings, x => x.MapFrom(GetPasswordSettings));
    }

    private static UsernameSettingsModel GetUsernameSettings(RealmEntity realm, RealmModel model)
    {
      var r = JsonSerializer.Deserialize<UsernameSettingsModel>(realm.UsernameSettings) ?? new();

      return r;
    }
    private static PasswordSettingsModel GetPasswordSettings(RealmEntity realm, RealmModel model)
    {
      var r = JsonSerializer.Deserialize<PasswordSettingsModel>(realm.PasswordSettings) ?? new();

      return r;
    }
  }
}
