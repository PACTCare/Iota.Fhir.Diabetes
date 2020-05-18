namespace Pact.App.Core.Chat
{
  using System.Windows.Input;

  using Telerik.XamarinForms.ConversationalUI;

  public class SuggestedActionCardContext : CardContext
  {
    public ICommand Command { get; set; }
    public string Text { get; set; }
  }
}