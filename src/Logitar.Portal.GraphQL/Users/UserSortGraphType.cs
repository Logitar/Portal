using GraphQL.Types;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.GraphQL.Users;

internal class UserSortGraphType : EnumerationGraphType<UserSort>
{
  public UserSortGraphType()
  {
    Name = nameof(UserSort);
    Description = "Represents the available user fields for sorting.";

    Add(UserSort.AuthenticatedOn, "The users will be their latest authentication date and time.");
    Add(UserSort.Birthdate, "The users will be their birthdate.");
    Add(UserSort.DisabledOn, "The users will be their latest disable date and time.");
    Add(UserSort.EmailAddress, "The users will be sorted by their email address.");
    Add(UserSort.FirstName, "The users will be sorted by their first name.");
    Add(UserSort.FullName, "The users will be sorted by their full name.");
    Add(UserSort.LastName, "The users will be sorted by their last name.");
    Add(UserSort.MiddleName, "The users will be sorted by their middle name.");
    Add(UserSort.Nickname, "The users will be sorted by their nickname.");
    Add(UserSort.PasswordChangedOn, "The users will be sorted by their password change date and time.");
    Add(UserSort.PhoneNumber, "The users will be sorted by their E.164 formatted phone number.");
    Add(UserSort.UniqueName, "The users will be sorted by their unique name.");
    Add(UserSort.UpdatedOn, "The users will be sorted by their latest update date and time.");
  }

  private void Add(UserSort value, string description) => Add(value.ToString(), value, description);
}
