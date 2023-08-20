using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal class ConfigurationProfile : Profile
{
  public ConfigurationProfile()
  {
    CreateMap<ReadOnlyUniqueNameSettings, UniqueNameSettings>();
    CreateMap<ReadOnlyPasswordSettings, PasswordSettings>();
  }
}
