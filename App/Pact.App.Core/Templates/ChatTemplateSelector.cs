namespace Pact.App.Core.Templates
{
  using Pact.App.Core.Chat;

  using Telerik.XamarinForms.ConversationalUI;

  using Xamarin.Forms;

  public class ChatTemplateSelector : ChatItemTemplateSelector
  {
    public DataTemplate ReceivedMessageTemplate { get; set; }
    public DataTemplate SentMessageTemplate { get; set; }
    public DataTemplate MicrosoftCardHero { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
      var chatItem = item as ChatItem;
      if (chatItem is TextMessage myItem)
      {
        return myItem.Author.Name == "Me" ? this.SentMessageTemplate : this.ReceivedMessageTemplate;
      }

      if (chatItem is PickerItem pickerItem && pickerItem.Context is MicrosoftCardHeroPickerContext)
      {
        return this.MicrosoftCardHero;
      }

      return base.OnSelectTemplate(item, container);
    }
  }
}