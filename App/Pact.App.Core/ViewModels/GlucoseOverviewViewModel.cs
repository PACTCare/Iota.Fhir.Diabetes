namespace Pact.App.Core.ViewModels
{
  using System;
  using System.Collections.ObjectModel;

  using Pact.App.Core.Models.Dexcom;
  using Pact.App.Core.Repository;

  using RestSharp;

  using Xamarin.Essentials;
  using Xamarin.Forms;

  public class GlucoseOverviewViewModel : BaseViewModel
  {
    public GlucoseOverviewViewModel()
    {
      this.GlucoseData = new ObservableCollection<GlucoseMeasurement>();
      this.Repository = new DexcomGlucoseManagementRepository(new RestClient("https://sandbox-api.dexcom.com"));
      this.SelectedDate = new DateTime(2015, 12, 12);
    }

    public ObservableCollection<GlucoseMeasurement> GlucoseData { get; set; }

    private string consentText;

    private Color consentColor;

    private DateTime selectedDate;

    public DateTime SelectedDate
    {
      get => this.selectedDate;
      set
      {
        this.selectedDate = value;
        this.OnPropertyChanged(nameof(this.SelectedDate));
      }
    }


    public string ConsentText
    {
      get => this.consentText;
      set
      {
        this.consentText = value;
        this.OnPropertyChanged(nameof(this.ConsentText));
      }
    }

    public Color ConsentColor
    {
      get => this.consentColor;
      set
      {
        this.consentColor = value;
        this.OnPropertyChanged(nameof(this.ConsentColor));
      }
    }

    private IGlucoseManagementRepository Repository { get; }

    public async void Init()
    {
      var share = await SecureStorage.GetAsync("SyncGlucose");
      if (string.IsNullOrEmpty(share) || !bool.Parse(share))
      {
        this.ConsentText = "Allow Data Upload";
        this.ConsentColor = Color.Green;
      }
      else
      {
        this.ConsentText = "Disallow Data Upload";
        this.ConsentColor = Color.Red;
      }
    }

    public async void Share()
    {
      var share = await SecureStorage.GetAsync("SyncGlucose");
      if (string.IsNullOrEmpty(share) || !bool.Parse(share))
      {
        await SecureStorage.SetAsync("SyncGlucose", true.ToString());

        this.ConsentText = "Disallow Data Upload";
        this.ConsentColor = Color.Red;
      }
      else
      {
        await SecureStorage.SetAsync("SyncGlucose", false.ToString());

        this.ConsentText = "Allow Data Upload";
        this.ConsentColor = Color.Green;
      }
    }

    public async void ChangeDate(DateTime newDate)
    {
      this.GlucoseData.Clear();
      this.SelectedDate = newDate;

      (await this.Repository.LoadGlucoseDataAsync(newDate, newDate.AddHours(24))).Egvs.ForEach(m => this.GlucoseData.Add(m));
    }

    public void SelectEarlierDay()
    {
      this.SelectedDate = this.SelectedDate.AddDays(-1);
    }

    public void SelectNextDay()
    {
      this.SelectedDate = this.SelectedDate.AddDays(1);
    }
  }
}