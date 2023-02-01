using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class SessionProfile : Profile
  {
    public SessionProfile()
    {
      CreateMap<SessionEntity, SessionModel>()
        .IncludeBase<AggregateEntity, AggregateModel>()
        .ForMember(x => x.SignedOutBy, x => x.Ignore()); // TODO(fpion): implement Actors
    }
  }
}
