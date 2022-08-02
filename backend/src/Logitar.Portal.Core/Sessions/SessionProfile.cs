using AutoMapper;
using Logitar.Portal.Core.Sessions.Models;

namespace Logitar.Portal.Core.Sessions
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
