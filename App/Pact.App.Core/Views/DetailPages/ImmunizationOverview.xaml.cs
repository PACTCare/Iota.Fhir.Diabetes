namespace Pact.App.Core.Views.DetailPages
{
  using System.Collections.Generic;

  using Pact.App.Core.Models;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ImmunizationOverview : ContentPage
  {
    public ImmunizationOverview(List<ImmunizationDetail> immunizationDetails)
    {
      this.InitializeComponent();
      this.ImmunizationDetails = immunizationDetails;

      this.BindingContext = this;
    }

    public List<ImmunizationDetail> ImmunizationDetails { get; set; }
  }
}