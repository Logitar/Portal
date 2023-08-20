using AutoMapper;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal class AggregateProfile : Profile
{
  public AggregateProfile()
  {
    CreateMap<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(y => y.AggregateId))
      .ForMember(x => x.CreatedBy, x => x.Ignore())
      .ForMember(x => x.CreatedOn, x => x.MapFrom(y => MappingHelper.AsUtc(y.CreatedOn)))
      .ForMember(x => x.UpdatedBy, x => x.Ignore())
      .ForMember(x => x.UpdatedOn, x => x.MapFrom(y => MappingHelper.AsUtc(y.UpdatedOn)));
  }
}
