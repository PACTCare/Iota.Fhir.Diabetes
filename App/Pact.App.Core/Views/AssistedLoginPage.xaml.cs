namespace Pact.App.Core.Views
{
  using System.Collections.Specialized;
  using System.Threading.Tasks;

  using Pact.App.Core.ViewModels;
  using Pact.App.Core.ViewModels.Chat;
  using Pact.Fhir.Iota.SqlLite;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  /// <summary>
  /// The assisted login page.
  /// </summary>
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AssistedLoginPage : ContentPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AssistedLoginPage"/> class.
    /// </summary>
    public AssistedLoginPage()
    {
      this.InitializeComponent();
      var viewModel = new AssistedLoginViewModel(this.Chat.Author, Application.Current.NavigationProxy, this.DisplayAlertMessage);

      this.BindingContext = viewModel;
      ((INotifyCollectionChanged)this.Chat.Items).CollectionChanged += viewModel.ChatItemsCollectionChanged;
    }

    private async Task DisplayAlertMessage(string arg1, string arg2)
    {
      await this.DisplayAlert(arg1, arg2, "OK", "NO");
    }
  }
}