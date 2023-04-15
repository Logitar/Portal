using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal class ResolveRecipientsHandler : IRequestHandler<ResolveRecipients, Recipients>
{
  private readonly IUserRepository _userRepository;

  public ResolveRecipientsHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Recipients> Handle(ResolveRecipients request, CancellationToken cancellationToken)
  {
    List<Guid> userIds = new(capacity: request.Recipients.Count());
    List<string> usernames = new(userIds.Capacity);
    foreach (RecipientInput input in request.Recipients)
    {
      if (input.User != null)
      {
        if (Guid.TryParse(input.User, out Guid userId))
        {
          userIds.Add(userId);
        }
        else
        {
          usernames.Add(input.User);
        }
      }
    }

    IEnumerable<UserAggregate> realmUsers = await _userRepository.LoadAsync(request.Realm, cancellationToken);
    Dictionary<Guid, UserAggregate> usersById = realmUsers.ToDictionary(x => x.Id.ToGuid(), x => x);
    Dictionary<string, UserAggregate> usersByUsername = realmUsers.ToDictionary(x => x.Username.ToUpper(), x => x);

    List<string> missingUsers = new(userIds.Capacity);
    List<Guid> missingEmails = new(userIds.Capacity);
    List<Recipient> to = new(userIds.Capacity);
    List<Recipient> cc = new(userIds.Capacity);
    List<Recipient> bcc = new(userIds.Capacity);
    foreach (RecipientInput input in request.Recipients)
    {
      UserAggregate? user = null;
      if (input.User != null)
      {
        if (Guid.TryParse(input.User, out Guid userId))
        {
          _ = usersById.TryGetValue(userId, out user);
        }

        if (user == null)
        {
          _ = usersByUsername.TryGetValue(input.User.ToUpper(), out user);
        }

        if (user == null)
        {
          missingUsers.Add(input.User);
          continue;
        }
        else if (user.Email == null)
        {
          missingEmails.Add(user.Id.ToGuid());
          continue;
        }
      }

      Recipient recipient = Recipient.From(input, user);

      switch (input.Type)
      {
        case RecipientType.Bcc:
          bcc.Add(recipient);
          break;
        case RecipientType.CC:
          cc.Add(recipient);
          break;
        case RecipientType.To:
          to.Add(recipient);
          break;
        default:
          throw new NotSupportedException($"The recipient type '{input.Type}' is not supported.");
      }
    }

    if (missingUsers.Any())
    {
      throw new UsersNotFoundException(missingUsers, request.ParamName);
    }

    if (missingEmails.Any())
    {
      throw new UsersHasNoEmailException(missingEmails, request.ParamName);
    }

    return new Recipients(to, cc, bcc);
  }
}
