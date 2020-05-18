namespace Pact.App.Core.Views.DetailPages
{
  using System.Collections.Generic;

  using Pact.App.Core.Models;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ConditionsOverview : ContentPage
  {
    public ConditionsOverview(List<ConditionDetail> conditionDetails)
    {
      this.InitializeComponent();
      this.ConditionDetails = conditionDetails;

      this.BindingContext = this;
    }

    public List<ConditionDetail> ConditionDetails { get; }
  }
}