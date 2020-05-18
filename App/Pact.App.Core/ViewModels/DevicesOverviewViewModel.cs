namespace Pact.App.Core.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  using Pact.App.Core.Models;
  using Pact.App.Core.Services.Authentication;
  using Pact.App.Core.Views.DetailPages;
  using Pact.App.Core.Views.Main;

  using Xamarin.Essentials;
  using Xamarin.Forms;

  public class DevicesOverviewViewModel : BaseViewModel
  {
    public DevicesOverviewViewModel(INavigation navigation)
      : base(navigation)
    {
      var device = SecureStorage.GetAsync("Device").Result;

      this.Devices = !string.IsNullOrEmpty(device)
                       ? new ObservableCollection<MedicalDevice>(new List<MedicalDevice> { new MedicalDevice { Name = device } })
                       : new ObservableCollection<MedicalDevice>();

      Authentication.DeviceAdded += this.AuthenticationOnDeviceAdded;
    }

    private ObservableCollection<MedicalDevice> devices;

    public ObservableCollection<MedicalDevice> Devices
    {
      get => this.devices;
      set
      {
        this.devices = value;
        this.OnPropertyChanged(nameof(this.Devices));
      }
    }

    public MedicalDevice SelectedDevice
    {
      get => null;
      set
      {
        if (value == null)
        {
          return;
        }

        Device.BeginInvokeOnMainThread(async () => await this.Navigation.PushModalAsync(new GlucoseOverview()));
      }
    }

    private async void AuthenticationOnDeviceAdded(object sender, EventArgs e)
    {
      await SecureStorage.SetAsync("Device", "Dexcom");

      Application.Current.MainPage = new TabbedMainNavigationPage();
    }
  }
}