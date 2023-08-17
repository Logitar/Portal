using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Configurations;

internal class ConfigurationProfile : Profile
{
  public ConfigurationProfile()
  {
    CreateMap<ConfigurationAggregate, Configuration>() // TODO(fpion): resolve actors
      .ForMember(x => x.Id, x => x.MapFrom(y => y.Id.Value))
      .ForMember(x => x.CreatedBy, x => x.Ignore())
      .ForMember(x => x.UpdatedBy, x => x.Ignore());
    CreateMap<ReadOnlyUniqueNameSettings, UniqueNameSettings>();
    CreateMap<ReadOnlyPasswordSettings, PasswordSettings>();
    CreateMap<ReadOnlyLoggingSettings, LoggingSettings>();
  }
}
