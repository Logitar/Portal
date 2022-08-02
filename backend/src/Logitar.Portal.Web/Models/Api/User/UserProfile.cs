using AutoMapper;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Web.Models.Api.User
{
  internal class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<UserModel, UserSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
