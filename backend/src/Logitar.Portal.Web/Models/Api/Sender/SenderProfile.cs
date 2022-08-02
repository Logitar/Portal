using AutoMapper;
using Logitar.Portal.Core.Emails.Senders.Models;

namespace Logitar.Portal.Web.Models.Api.Sender
{
  internal class SenderProfile : Profile
  {
    public SenderProfile()
    {
      CreateMap<SenderModel, SenderSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
