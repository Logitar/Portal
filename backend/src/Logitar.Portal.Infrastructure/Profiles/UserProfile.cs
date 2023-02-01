using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users.Models;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<UserEntity, UserModel>()
        .IncludeBase<AggregateEntity, AggregateModel>();
    }
  }
}
