using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal.Web
{
  internal class HttpUserContext : IUserContext
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext HttpContext => _httpContextAccessor.HttpContext
      ?? throw new InvalidOperationException($"The {nameof(HttpContext)} is required.");

    public ActorModel Actor
    {
      get
      {
        UserModel? user = HttpContext.GetUser();
        if (user != null)
        {
          return new ActorModel
          {
            Id = user.Id,
            Type = ActorType.User,
            DisplayName = user.FullName ?? user.Username,
            Email = user.Email,
            Picture = user.Picture
          };
        }

        //ApiKey? apiKey = HttpContext.GetApiKey();
        //if (apiKey != null)
        //{
        //  throw new NotImplementedException(); // TODO(fpion): implement
        //}

        return new ActorModel
        {
          Id = "SYSTEM",
          DisplayName = "System"
        };
      }
    }
    public AggregateId ActorId => new(Actor.Id);

    public string SessionId => HttpContext.GetSession()?.Id ?? throw new InvalidOperationException("The Session is required.");
    public string UserId => HttpContext.GetUser()?.Id ?? throw new InvalidOperationException("The User is required.");

    public string BaseUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
  }
}
