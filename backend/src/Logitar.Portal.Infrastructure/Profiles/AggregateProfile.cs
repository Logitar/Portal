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
        .ForMember(x => x.Id, x => x.MapFrom(y => y.AggregateId));
    }
  }
}
