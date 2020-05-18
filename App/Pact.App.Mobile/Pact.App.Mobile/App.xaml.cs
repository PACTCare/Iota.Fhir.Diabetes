namespace Pact.App.Mobile
{
  using System;
  using System.IO;

  using Pact.App.Core.Views;
  using Pact.App.Core.Views.Main;

  using Xamarin.Essentials;
  using Xamarin.Forms;

  /// <summary>
  /// The app.
  /// </summary>
  public partial class App : Application
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
      this.InitializeComponent();
      if (!string.IsNullOrEmpty(SecureStorage.GetAsync("LoggedIn").Result))
      {
        this.MainPage = new TabbedMainNavigationPage();
      }
      else
      {
        this.MainPage = new AssistedLoginPage();
      }
    }

    /// <summary>
    /// The on resume.
    /// </summary>
    protected override void OnResume()
    {
      // Handle when your app resumes
    }

    /// <summary>
    /// The on sleep.
    /// </summary>
    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }
  }
}