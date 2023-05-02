using AutoMapper;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Profiles;

internal class ConfigurationProfile : Profile
{
  public ConfigurationProfile()
  {
    CreateMap<ConfigurationAggregate, Configuration>()
      .ForMember(x => x.CreatedBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.CreatedById)))
      .ForMember(x => x.UpdatedBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.UpdatedById)));
    CreateMap<ReadOnlyLoggingSettings, LoggingSettings>();
    CreateMap<ReadOnlyPasswordSettings, PasswordSettings>();
    CreateMap<ReadOnlyUsernameSettings, UsernameSettings>();
  }
}
