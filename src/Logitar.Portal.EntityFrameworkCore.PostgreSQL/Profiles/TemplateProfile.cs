using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Profiles;

internal class TemplateProfile : Profile
{
  public TemplateProfile()
  {
    CreateMap<TemplateEntity, Template>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.GetId))
      .ForMember(x => x.Key, x => x.MapFrom(y => y.UniqueName));
  }
}
