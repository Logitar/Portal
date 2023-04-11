using AutoMapper;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Templates;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Profiles;

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
