namespace Pact.App.Core.ViewModels
{
  using System.Collections.Generic;

  using Pact.App.Core.Entity;
  using Pact.App.Core.Navigation;

  using Telerik.XamarinForms.DataGrid;

  using Xamarin.Forms;

  /// <summary>
  ///   The dashboard view model.
  /// </summary>
  public class DashboardViewModel : BaseViewModel
  {
    public DashboardViewModel(INavigation navigation)
      : base(navigation)
    {
    }

    // ReSharper disable once UnusedMember.Global
    public List<HealthSection> SectionList =>
      new List<HealthSection>
        {
          new HealthSection { SectionText = "Profile", Type = HealthSectionType.Patient, ImagePath = "profile.png" },
          new HealthSection { SectionText = "Vaccinations", Type = HealthSectionType.Immunization, ImagePath = "vaccination.png"} ,
          new HealthSection { SectionText = "Conditions", Type = HealthSectionType.Conditions, ImagePath = "observations.png" },
          new HealthSection { SectionText = "My Devices", Type = HealthSectionType.Devices, ImagePath = "my_devices.png" },
          new HealthSection { SectionText = "Shared Data", Type = HealthSectionType.Shared, ImagePath = "shared_data.png" }
        };

    // ReSharper disable once UnusedMember.Global
    public HealthSection SelectedSection
    {
      get => null;
      set
      {
        if (value == null)
        {
          return;
        }

        this.ResolveHealthSectionAsync(value);
      }
    }

    public async void ResolveHealthSectionAsync(HealthSection healthSection)
    {
      var contentPage = await HealthSectionResolver.ResolveAsync(healthSection);
      if (contentPage == null)
      {
        return;
      }

      Device.BeginInvokeOnMainThread(async () => await this.Navigation.PushModalAsync(contentPage));
      this.OnPropertyChanged(nameof(this.SelectedSection));
    }
  }
}