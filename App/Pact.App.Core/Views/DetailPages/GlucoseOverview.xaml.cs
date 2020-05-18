namespace Pact.App.Core.Views.DetailPages
{
  using System;

  using Pact.App.Core.Models;
  using Pact.App.Core.ViewModels;

  using Telerik.XamarinForms.Input;

  using Xamarin.Essentials;
  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class GlucoseOverview : ContentPage
  {
    public GlucoseOverview()
    {
      this.InitializeComponent();
      this.BindingContext = new GlucoseOverviewViewModel();
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
      ((GlucoseOverviewViewModel)this.BindingContext).Init();
      base.OnAppearing();
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
      if (sender is RadButton)
      {
        ((GlucoseOverviewViewModel)this.BindingContext).Share();
      }
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
      ((GlucoseOverviewViewModel)this.BindingContext).ChangeDate(e.NewDate);
    }

    private void SelectEarlierDayClicked(object sender, EventArgs e)
    {
      ((GlucoseOverviewViewModel)this.BindingContext).SelectEarlierDay();
    }

    private void SelectNextDayClicked(object sender, EventArgs e)
    {
      ((GlucoseOverviewViewModel)this.BindingContext).SelectNextDay();
    }
  }
}