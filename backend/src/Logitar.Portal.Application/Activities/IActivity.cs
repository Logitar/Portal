namespace Logitar.Portal.Application.Activities;

public interface IActivity
{
  IActivity Anonymize();
  void Contextualize(ActivityContext context);
}
