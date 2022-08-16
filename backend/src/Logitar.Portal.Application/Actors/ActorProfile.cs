using AutoMapper;
using Logitar.Portal.Core.Actors.Models;
using Logitar.Portal.Domain.Actors;

namespace Logitar.Portal.Application.Actors
{
  internal class ActorProfile : Profile
  {
    public ActorProfile()
    {
      CreateMap<Actor, ActorModel>();
    }
  }
}
