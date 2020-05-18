namespace Pact.App.Core.Views.DetailPages
{
  using System;

  using Pact.App.Core.Services.Authentication;
  using Pact.App.Core.ViewModels;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class DevicesOverview : ContentPage
  {
    public DevicesOverview()
    {
      this.InitializeComponent();
      this.BindingContext = new DevicesOverviewViewModel(this.Navigation);
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
      Authentication.Login();
    }
  }
}