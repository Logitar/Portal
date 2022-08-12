using AutoMapper;
using Logitar.Portal.Core.Actors.Models;

namespace Logitar.Portal.Core.Actors
{
  internal class ActorProfile : Profile
  {
    public ActorProfile()
    {
      CreateMap<Actor, ActorModel>();
    }
  }
}
