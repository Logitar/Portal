using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class AggregateProfile : Profile
  {
    public AggregateProfile()
    {
      CreateMap<AggregateEntity, AggregateModel>()
        .ForMember(x => x.Id, x => x.MapFrom(y => y.AggregateId))
        .ForMember(x => x.CreatedBy, x => x.Ignore()) // TODO(fpion): implement Actors
        .ForMember(x => x.UpdatedBy, x => x.Ignore()); // TODO(fpion): implement Actors
    }
  }
}
