using GraphQL.Types;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.GraphQL;

internal abstract class SortOptionInputGraphType<T> : InputObjectGraphType<T> where T : SortOption
{
  public SortOptionInputGraphType(bool registerField = false)
  {
    Name = typeof(T).Name;
    Description = "Represents a sort option parameter.";

    if (registerField)
    {
      Field(x => x.Field)
        .Description("The field on which to apply the sort.");
    }

    Field(x => x.IsDescending).DefaultValue(false)
      .Description("A value indicating whether or not the sort is descending.");
  }
}
