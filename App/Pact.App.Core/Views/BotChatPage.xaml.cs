namespace Pact.App.Core.Views
{
  using System.Collections.Specialized;
  using System.Linq;

  using Pact.App.Core.Models;
  using Pact.App.Core.ViewModels.Chat;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class BotChatPage : ContentPage
  {
    public BotChatPage()
    {
      this.InitializeComponent();
      var viewModel = new BotChatViewModel(
        new Bot
          {
            Name = "Florence",
            DirectLineSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxx", // <= your direct line secret
            Avatar = "https://florenceblob.blob.core.windows.net/thumbnails/final_verysmall2.png"
          },
        Application.Current.NavigationProxy,
        this.Chat.Author);

      this.BindingContext = viewModel;
      ((INotifyCollectionChanged)this.Chat.Items).CollectionChanged += viewModel.ChatItemsCollectionChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
      await ((BotChatViewModel)this.BindingContext).InitAsync();
      base.OnAppearing();
    }
  }
}