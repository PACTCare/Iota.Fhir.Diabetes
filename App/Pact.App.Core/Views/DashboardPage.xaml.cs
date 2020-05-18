namespace Pact.App.Core.Views
{
  using System;
  using System.IO;

  using Pact.App.Core.Services;
  using Pact.App.Core.Services.Authentication;
  using Pact.App.Core.ViewModels;

  using Xamarin.Essentials;
  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  /// <summary>
  /// The dashboard page.
  /// </summary>
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class DashboardPage : ContentPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardPage"/> class.
    /// </summary>
    public DashboardPage()
    {
      this.InitializeComponent();
      this.BindingContext = new DashboardViewModel(this.Navigation);
    }

    private async void OnLogout(object sender, EventArgs e)
    {
      await DependencyResolver.Resolve<ILogout>().LogoutAsync();
      Application.Current.MainPage = new AssistedLoginPage();
    }
  }
} 