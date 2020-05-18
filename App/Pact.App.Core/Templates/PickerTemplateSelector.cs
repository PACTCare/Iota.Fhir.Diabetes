namespace Pact.App.Core.Templates
{
  using Pact.App.Core.Chat;
  using Pact.App.Core.ViewModels.Chat;

  using Telerik.XamarinForms.ConversationalUI;

  using Xamarin.Forms;

  public class PickerTemplateSelector : ChatPickerTemplateSelector
  {
    public DataTemplate SuggestedActionTemplate { get; set; }

    /// <inheritdoc />
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
      if (item is SuggestedActionPickerContext)
      {
        return this.SuggestedActionTemplate;
      }

      return base.OnSelectTemplate(item, container);
    }
  }
}