using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application
{
  internal class AggregateProfile : Profile
  {
    public AggregateProfile()
    {
      CreateMap<Aggregate, AggregateModel>();
      CreateMap<AggregateModel, AggregateSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt))
        .ForMember(x => x.UpdatedBy, x => x.MapFrom(y => y.UpdatedBy ?? y.CreatedBy));
    }
  }
}
