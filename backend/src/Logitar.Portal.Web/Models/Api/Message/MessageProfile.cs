using AutoMapper;
using Logitar.Portal.Core.Emails.Messages.Models;

namespace Logitar.Portal.Web.Models.Api.Message
{
  internal class MessageProfile : Profile
  {
    public MessageProfile()
    {
      CreateMap<MessageModel, MessageSummary>()
        .ForMember(x => x.Recipients, x => x.MapFrom(y => y.Recipients.Count()))
        .ForMember(x => x.SentAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
