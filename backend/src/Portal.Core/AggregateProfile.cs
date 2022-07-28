using AutoMapper;

namespace Portal.Core
{
  internal class AggregateProfile : Profile
  {
    public AggregateProfile()
    {
      CreateMap<Aggregate, AggregateModel>();
    }
  }
}
