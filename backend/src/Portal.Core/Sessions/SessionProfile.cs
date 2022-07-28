using AutoMapper;
using Portal.Core.Sessions.Models;

namespace Portal.Core.Sessions
{
  internal class SessionProfile : Profile
  {
    public SessionProfile()
    {
      CreateMap<Session, SessionModel>()
        .IncludeBase<Aggregate, AggregateModel>();
    }
  }
}
