using AutoMapper;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Profiles;

internal class ConfigurationProfile : Profile
{
  public ConfigurationProfile()
  {
    CreateMap<ConfigurationAggregate, Configuration>();
    CreateMap<ReadOnlyLoggingSettings, LoggingSettings>();
    CreateMap<ReadOnlyPasswordSettings, PasswordSettings>();
    CreateMap<ReadOnlyUsernameSettings, UsernameSettings>();
  }
}
