using AutoMapper;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Profiles;

internal class AggregateProfile : Profile
{
  public AggregateProfile()
  {
    CreateMap<AggregateEntity, Aggregate>()
      .ForMember(x => x.CreatedBy, x => x.MapFrom(y => MappingHelper.GetActor(y.CreatedById, y.CreatedBy)))
      .ForMember(x => x.UpdatedBy, x => x.MapFrom(y => MappingHelper.GetActor(y.UpdatedById ?? y.CreatedById, y.UpdatedBy ?? y.CreatedBy)))
      .ForMember(x => x.UpdatedOn, x => x.MapFrom(y => y.UpdatedOn ?? y.CreatedOn));
  }
}
