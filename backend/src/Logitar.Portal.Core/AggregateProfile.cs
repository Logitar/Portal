using AutoMapper;

namespace Logitar.Portal.Core
{
  internal class AggregateProfile : Profile
  {
    public AggregateProfile()
    {
      CreateMap<Aggregate, AggregateModel>();
    }
  }
}
